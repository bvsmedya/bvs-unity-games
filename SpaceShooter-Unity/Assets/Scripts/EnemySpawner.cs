using UnityEngine;

/// KURULUM:
/// 1. Bos GameObject olustur, adi 'EnemySpawner'
/// 2. Bu scripti ekle
/// 3. Inspector'dan enemyPrefab'i bagla
/// 4. SpawnY: kameranin ustu (yaklasik 6)

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefablar")]
    [SerializeField] private GameObject enemyPrefab;

    [Header("Spawn Ayarlari")]
    [SerializeField] private float initialSpawnRate = 2f;
    [SerializeField] private float minSpawnRate = 0.4f;
    [SerializeField] private float difficultyIncreaseInterval = 15f;
    [SerializeField] private float spawnY = 6f;

    private float currentSpawnRate;
    private float nextSpawnTime;
    private float screenHalfWidth;

    private void Start()
    {
        currentSpawnRate = initialSpawnRate;
        screenHalfWidth = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x - 1f;
        InvokeRepeating(nameof(IncreaseDifficulty), difficultyIncreaseInterval, difficultyIncreaseInterval);
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + currentSpawnRate;
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab == null) return;
        float x = Random.Range(-screenHalfWidth, screenHalfWidth);
        Vector3 spawnPos = new Vector3(x, spawnY, 0);
        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }

    private void IncreaseDifficulty()
    {
        currentSpawnRate = Mathf.Max(minSpawnRate, currentSpawnRate * 0.8f);
        Debug.Log("Zorluk artti! Spawn rate: " + currentSpawnRate);
    }
}