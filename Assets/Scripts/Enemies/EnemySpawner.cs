using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private SpawnerConfig config;

    [Header("Puntos de spawn")]
    public Transform[] spawnPoints;

    private int currentEnemies = 0;
    private bool playerInside = false;

    private Coroutine spawnCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;

            if (spawnCoroutine == null)
                spawnCoroutine = StartCoroutine(SpawnLoop());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;

            if (spawnCoroutine != null)
            {
                StopCoroutine(spawnCoroutine);
                spawnCoroutine = null;
            }
        }
    }

    IEnumerator SpawnLoop()
    {
        while (playerInside)
        {
            if (currentEnemies < config.maxEnemies)
            {
                SpawnEnemy();
            }

            yield return new WaitForSeconds(config.spawnInterval);
        }
    }

    void SpawnEnemy()
    {
        if (spawnPoints.Length == 0) return;

        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject enemy = Instantiate(config.enemyPrefab, point.position, Quaternion.identity);

        currentEnemies++;

        
        EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.OnEnemyDied += OnEnemyDied;
        }
    }

    void OnEnemyDied()
    {
        currentEnemies--;
    }
}