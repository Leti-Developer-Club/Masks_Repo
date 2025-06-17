using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Header("Enemy Types")]
    [Tooltip("Drag allEnemies possible enemy prefabs here")]
    public List<GameObject> allEnemyPrefabs;

    [Header("Platform Setup")]
    [Tooltip("All platforms that must be filled to win")]
    public List<WisdomPlatform> platforms;

    [Header("Spawn Points")]
    public List<Transform> spawnPoints;

    [Header("Wave Settings")]
    [SerializeField] private int baseTypeCount = 3;
    [SerializeField] private int typeCountIncrementPerWave = 1;
    [SerializeField] private int baseMaxEnemies = 5;
    [SerializeField] private int maxEnemiesIncrementPerWave = 2;
    [SerializeField] private float breakBetweenWaves = 5f;

    private int waveIndex = 0;
    private int currentMaxTypes;
    private int currentMaxEnemies;
    private bool spawning = false;
    public BreakOverlayManager breakOverlayManager;

    [Header("UI Shandis")]
    [SerializeField] private TMP_Text timeToNextWave;

    private void Start()
    {
        foreach (var p in platforms)
            p.onPlatformFull.AddListener(PlatformFull_StopSpawning);//when platform is full stop spawning

        StartCoroutine(WaveRoutine());
    }
 
    private void PlatformFull_StopSpawning()
    {
        spawning = false;
        ClearAllEnemies();
    }
    private void ClearAllEnemies()
    {
        var allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in allEnemies)
            Destroy(enemy);
    }

    private IEnumerator WaveRoutine()
    {
        while (true)
        {
            // check win
            bool allFull = true;
            foreach (var p in platforms)
            {
                if (p == null || !p.IsFull)
                {
                    allFull = false;
                    break;
                }
            }
            if (allFull)
            {
                print("won");
            }

            // wave settings
            currentMaxTypes = baseTypeCount + waveIndex * typeCountIncrementPerWave;
            currentMaxEnemies = baseMaxEnemies + waveIndex * maxEnemiesIncrementPerWave;

            spawning = true;
            List<GameObject> waveTypes = PickRandomTypes(currentMaxTypes);

            // spawn loop
            while (spawning)
            {
                // count and cleanup
                int activeEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
                if (activeEnemies < currentMaxEnemies)
                    SpawnOne(waveTypes);

                yield return null;
            }

            timeToNextWave.gameObject.SetActive(true);
            breakOverlayManager.StartBreakOverlay(3f);
            float timer = breakBetweenWaves;
            while (timer > 0f)
            {
                if (timeToNextWave != null)
                    timeToNextWave.text = Mathf.CeilToInt(timer).ToString();

                timer -= Time.deltaTime;
                yield return null;
            }
            waveIndex++;
            timeToNextWave.gameObject.SetActive(false);
        }
    }

    private List<GameObject> PickRandomTypes(int count)
    {
        List<GameObject> result = new List<GameObject>();
        var pool = new List<GameObject>(allEnemyPrefabs);
        for (int i = 0; i < count && pool.Count > 0; i++)
        {
            int idx = Random.Range(0, pool.Count);
            result.Add(pool[idx]);
            pool.RemoveAt(idx);
        }
        return result;
    }

    private void SpawnOne(List<GameObject> types)
    {
        if (types.Count == 0 || spawnPoints.Count == 0) return;

        var prefab = types[Random.Range(0, types.Count)];
        var spawn = spawnPoints[Random.Range(0, spawnPoints.Count)];
        Instantiate(prefab, spawn.position, Quaternion.identity);
    }
}

