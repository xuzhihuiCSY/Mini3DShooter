using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 5f;
    public float spawnDistance = 15f;
    public float safeDistanceFromPlayer = 6f;
    public int maxEnemies = 3;

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        InvokeRepeating("SpawnEnemy", 2f, spawnInterval);
    }

    void SpawnEnemy()
    {
        if (player == null) return;

        // Count existing enemies
        int currentEnemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        if (currentEnemyCount >= maxEnemies)
        {
            return;
        }

        float groundHalfSize = 12.5f;
        Vector3 spawnPos;
        int maxAttempts = 3;
        int attempts = 0;

        do
        {
            float x = Random.Range(-groundHalfSize, groundHalfSize);
            float z = Random.Range(-groundHalfSize, groundHalfSize);
            spawnPos = new Vector3(x, player.position.y + 2f, z);
            attempts++;
        } while (Vector3.Distance(spawnPos, player.position) < safeDistanceFromPlayer && attempts < maxAttempts);

        UnityEngine.AI.NavMeshHit hit;
        if (UnityEngine.AI.NavMesh.SamplePosition(spawnPos, out hit, 10f, UnityEngine.AI.NavMesh.AllAreas))
        {
            GameObject enemy = Instantiate(enemyPrefab, hit.position, Quaternion.identity);
            enemy.tag = "Enemy"; // ✅ Make sure the prefab has this tag
        }
        else
        {
            Debug.LogWarning("Could not find valid NavMesh spot to spawn enemy.");
        }
    }
}
