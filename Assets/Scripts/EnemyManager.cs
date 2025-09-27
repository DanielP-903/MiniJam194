using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    // Vans (enemy spawners)
    [SerializeField] private List<VanSpawnPoint> vanSpawnPoints;
    [SerializeField] private Van vanPrefab;
    [SerializeField] private int VAN_POOL_SIZE = 4;
    [SerializeField] private float VAN_SPAWN_DELAY = 5.0f;
    [SerializeField] private int VAN_MAX_SPAWN_AMOUNT = 4;
    
    // Enemies
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private int ENEMY_POOL_SIZE = 10;
    [SerializeField] private float ENEMY_SPAWN_DELAY = 2.0f;
    [SerializeField] private int ENEMY_MAX_SPAWN_AMOUNT = 3;

    private float lastEnemySpawnTime;
    private List<Enemy> enemyPool = new List<Enemy>();
    private int enemiesAlive;

    private float lastVanSpawnTime;
    private List<Van> vanPool = new List<Van>();
    private int vansAlive;
    
    private Dictionary<VanSpawnPoint, bool> vanSpawnPointStatuses = new Dictionary<VanSpawnPoint, bool>();
    
    private void Awake()
    {
        for (int i = 0; i < ENEMY_POOL_SIZE; i++)
        {
            Enemy newEnemy = Instantiate(enemyPrefab);
            newEnemy.Deactivate();
            enemyPool.Add(newEnemy);
        }
        
        lastEnemySpawnTime = Time.time;

        for (int i = 0; i < VAN_POOL_SIZE; i++)
        {
            Van newVan = Instantiate(vanPrefab);
            newVan.Deactivate();
            vanPool.Add(newVan);
        }
        
        lastVanSpawnTime = Time.time;

        foreach (var spawnPoint in vanSpawnPoints)
        {
            vanSpawnPointStatuses.Add(spawnPoint, false);
        }
    }

    private void Update()
    {
        UpdateVans();
        if (vanSpawnPointStatuses.Count == 0)
        {
            return;
        }

        UpdateEnemies();
    }

    private void UpdateVans()
    {
        if (vansAlive >= VAN_MAX_SPAWN_AMOUNT || !(Time.time - lastVanSpawnTime > VAN_SPAWN_DELAY))
        {
            return;
        }
        
        //print("Attempting to spawn a van");
        
        lastVanSpawnTime = Time.time;

        VanSpawnPoint spawnPoint = GetRandomAvailableVanSpawnPoint();
        if (!spawnPoint)
        {
            // No valid spawn point try again next time
            //print("No valid spawn point for a van found!");
            return;
        }
        
        print("Successfully spawned a van");
        SpawnVan(spawnPoint);
    }

    private void UpdateEnemies()
    {
        if (enemiesAlive >= ENEMY_MAX_SPAWN_AMOUNT || !(Time.time - lastEnemySpawnTime > ENEMY_SPAWN_DELAY))
        {
            return;
        }

        Van randomVan = GetRandomActiveVan();
        if (!randomVan)
        {
            return;
        }
        
        lastEnemySpawnTime = Time.time;
        SpawnEnemy(randomVan.GetRandomSpawnLocation());
    }

    private VanSpawnPoint GetRandomAvailableVanSpawnPoint()
    {
        List<VanSpawnPoint> spawnPoints = new List<VanSpawnPoint>();
        foreach (KeyValuePair<VanSpawnPoint, bool> van in vanSpawnPointStatuses)
        {
            if (!van.Value)
            {
                spawnPoints.Add(van.Key);
            }
        }
        return spawnPoints.Count == 0 ? null : spawnPoints[Random.Range(0, spawnPoints.Count)];
    }

    private Van GetRandomActiveVan()
    {
        List<Van> activeVans = new List<Van>();
        foreach (Van van in vanPool)
        {
            if (van.GetActive() && van.CanSpawnEnemies())
            {
                activeVans.Add(van);
            }
        }

        return activeVans.Count == 0 ? null : activeVans[Random.Range(0, activeVans.Count)];
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
    
    private Van GetFirstAvailableVanFromPool()
    {
        foreach (var van in vanPool)
        {
            if (van.GetActive()) 
                continue;
            
            // Found an inactive van
            van.Activate();
            return van;
        }
        
        // None active... make new one
        
        Van newVan = Instantiate(vanPrefab, transform);
        newVan.Activate();
        vanPool.Add(newVan);
        return newVan;
    }

    private void SpawnVan(VanSpawnPoint spawnPointObject)
    {
        Van spawnedVan = GetFirstAvailableVanFromPool();
        vanSpawnPointStatuses[spawnPointObject] = true;
        spawnedVan.SetSpawnPointObject(spawnPointObject);
        spawnedVan.transform.position = new Vector3(
            spawnPointObject.transform.position.x + spawnPointObject.GetComponent<VanSpawnPoint>().GetVanSpawnOffsetX(),
            spawnPointObject.transform.position.y,
            0.0f);
        vansAlive++;
    }

    public void OnVanDestroyed(Van van)
    {
        vanSpawnPointStatuses[van.GetSpawnPointObject()] = false;
    }
    
    public void OnEnemyDestroyed()
    {
        enemiesAlive--;
    }
}
