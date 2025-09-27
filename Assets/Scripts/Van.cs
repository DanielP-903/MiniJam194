using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Van : Tile
{
    public enum EVanState
    {
        Entering,
        Idle,
        Dead
    }
    
    [SerializeField] private List<GameObject> spawnLocations;
    [SerializeField] private float VAN_ENTER_SPEED = 3.0f;

    private EnemyManager enemyManager;
    private EVanState vanState = EVanState.Entering;
    private Vector3 vanDestination;
    
    private VanSpawnPoint spawnPointObject;
    

    private void Awake()
    {
        Init();
        enemyManager = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();
        active = false;
    }
    
    public Vector3 GetRandomSpawnLocation()
    {
        return transform.position + spawnLocations[Random.Range(0, spawnLocations.Count)].transform.localPosition;
    }

    private void Update()
    {
        if (!active)
        {
            return;
        }

        switch (vanState)
        {
            case EVanState.Entering:
                transform.position = Vector3.MoveTowards(transform.position, vanDestination, VAN_ENTER_SPEED * Time.deltaTime);
                if (transform.position == vanDestination)
                {
                    vanState = EVanState.Idle;
                }
                break;
            case EVanState.Idle:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public bool CanSpawnEnemies()
    {
        return vanState == EVanState.Idle && !IsAtMaxState();
    }
    
    public void Activate()
    {
        active = true;
        vanState = EVanState.Entering;
        UpdateState(initialState);
        gameObject.SetActive(true);
    }
    
    public void Deactivate()
    {
        active = false;
        gameObject.SetActive(false);
    }

    public bool GetActive()
    {
        return active;
    }

    public void SetSpawnPointObject(VanSpawnPoint newSpawnPointObject)
    {
        spawnPointObject = newSpawnPointObject;
        vanDestination = spawnPointObject.transform.position;

        // Ensure we are facing the right way
        spriteRenderer.flipX = newSpawnPointObject.GetVanSpawnOffsetX() < 0;
    }

    public VanSpawnPoint GetSpawnPointObject()
    {
        return spawnPointObject;
    }

    protected override void OnReachedMaxState()
    {
        base.OnReachedMaxState();
        vanState = EVanState.Dead;
        Deactivate();
        enemyManager.OnVanDestroyed(this);
    }
}
