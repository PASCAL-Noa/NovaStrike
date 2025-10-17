using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnInterval = 2f; // intervalle de base
    [SerializeField] private float spawnRangeX = 8f;

    private float baseInterval;
    private float nextSpawnTime;

    private void Awake()
    {
        baseInterval = spawnInterval;
        nextSpawnTime = Time.time + spawnInterval;
    }

    private void OnEnable()
    {
        // recalculer au cas où SetSpawnMultiplier a été appelé avant activation
        nextSpawnTime = Time.time + spawnInterval;
    }

    private void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    public void SetSpawnMultiplier(float multiplier)
    {
        // multiplier <1 -> spawn plus rapide (intervalle réduit)
        spawnInterval = baseInterval * multiplier;
        // adapte le nextSpawnTime pour éviter un saut instantané
        nextSpawnTime = Time.time + spawnInterval;
    }

    void SpawnEnemy()
    {
        float x = Random.Range(-spawnRangeX, spawnRangeX);
        Vector3 spawnPos = new Vector3(x, transform.position.y, transform.position.z);
        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }
}