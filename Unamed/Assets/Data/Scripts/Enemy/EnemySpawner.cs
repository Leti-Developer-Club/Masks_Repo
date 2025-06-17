using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private int maxEnemies = 10;

    [Header("Spawn Points")]
    [SerializeField] private List<Transform> spawnPoints;

    private float nextSpawnTime;
    private List<GameObject> spawnedEnemies = new();

    void Update()
    {
        // Clean up null (destroyed) enemies
        spawnedEnemies.RemoveAll(enemy => enemy == null);

        if (Time.time >= nextSpawnTime && spawnedEnemies.Count < maxEnemies)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + spawnInterval;
            print("Spawned");
        }
    }

    void SpawnEnemy()
    {
        if (spawnPoints.Count == 0)
        {
            Debug.LogWarning("No spawn points assigned to EnemySpawner!");
            return;
        }

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        spawnedEnemies.Add(enemy);
    }
}

