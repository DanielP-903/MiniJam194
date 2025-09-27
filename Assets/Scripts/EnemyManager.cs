using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private List<Van> vans;
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private int ENEMY_POOL_SIZE = 10;
    [SerializeField] private float ENEMY_SPAWN_DELAY = 2.0f;
    [SerializeField] private int ENEMY_MAX_SPAWN_AMOUNT = 3;

    private float lastSpawnTime;
    private List<Enemy> enemyPool = new List<Enemy>();

    private int enemiesAlive;
    
    private void Awake()
    {
        for (int i = 0; i < ENEMY_POOL_SIZE; i++)
        {
            Enemy newEnemy = Instantiate(enemyPrefab);
            newEnemy.Deactivate();
            enemyPool.Add(newEnemy);
        }
        
        lastSpawnTime = Time.time;
    }

    private void Update()
    {
        if (vans.Count == 0)
        {
            return;
        }
        
        if (enemiesAlive < ENEMY_MAX_SPAWN_AMOUNT && Time.time - lastSpawnTime > ENEMY_SPAWN_DELAY)
        {
            lastSpawnTime = Time.time;
            SpawnEnemy(vans[Random.Range(0, vans.Count)].GetRandomSpawnLocation());
        }
    }

    private Enemy GetFirstAvailableEnemyFromPool()
    {
        foreach (var enemy in enemyPool)
        {
            if (enemy.GetActive()) 
                continue;
            
            // Found an inactive enemy
            enemy.Activate();
            return enemy;
        }
        
        // None active... make new one
        
        Enemy newEnemy = Instantiate(enemyPrefab, transform);
        newEnemy.Activate();
        enemyPool.Add(newEnemy);
        return newEnemy;
    }

    private void SpawnEnemy(Vector3 position)
    {
        Enemy spawnedEnemy = GetFirstAvailableEnemyFromPool();
        spawnedEnemy.transform.position = position;
        enemiesAlive++;
    }
}
