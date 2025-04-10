using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 1;

    void Start()
    {
        Debug.Log("Bullet spawned: " + gameObject.name);
    }


    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Bullet hit: " + collision.collider.name);

        Enemy enemy = collision.collider.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        Destroy(gameObject);
    }

}
