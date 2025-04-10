using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public int health = 3;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    public int attackDamage = 1;

    private Transform player;
    private NavMeshAgent agent;
    private Animation anim;
    private bool isDead = false;
    private float lastAttackTime = 0f;
    private float repathTimer = 0f;

    private string[] attackAnimations = new string[]
    {
        "attack-kick-right",
        "attack-kick-left",
        "attack-melee-right",
        "attack-melee-left"
    };

    void Start()
    {
        anim = GetComponent<Animation>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning(gameObject.name + " not placed on NavMesh!");
            return;
        }

        if (anim != null)
        {
            anim.Play("walk");
        }
    }

    void Die()
    {
        isDead = true;

        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        if (anim != null)
        {
            anim.Play("die");
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        Destroy(gameObject, 2f);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        health -= damage;
        if (health <= 0) Die();
    }

    void Update()
    {
        if (isDead || agent == null || !agent.enabled || player == null) return;

        if (agent.isOnNavMesh)
        {
            float distance = Vector3.Distance(transform.position, player.position);

            if (distance <= attackRange)
            {
                agent.isStopped = true;
                FacePlayer();
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    AttackPlayer();
                    lastAttackTime = Time.time;
                }
            }
            else
            {
                agent.isStopped = false;

                repathTimer += Time.deltaTime;
                if (repathTimer >= 1f || !agent.hasPath)
                {
                    agent.SetDestination(player.position);
                    repathTimer = 0f;
                }
                if (Time.time >= lastAttackTime + attackCooldown && !anim.IsPlaying("walk"))
                {
                    anim.CrossFade("walk");
                }
            }
        }
    }

    void AttackPlayer()
    {
        Debug.Log(gameObject.name + " attacks the player!");
        if (anim != null && attackAnimations.Length > 0)
        {
            string randomAttack = attackAnimations[Random.Range(0, attackAnimations.Length)];
            anim.Play(randomAttack);
        }

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
        }
    }

    void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0f; // Keep only horizontal rotation
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        }
    }

}
