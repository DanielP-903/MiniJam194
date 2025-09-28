using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Van : Tile
{
    private enum EVanState
    {
        Entering,
        Idle,
        Dead
    }
    
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private List<GameObject> spawnLocations;
    [SerializeField] private float VAN_ENTER_SPEED = 3.0f;

    private BoxCollider2D boxCollider;
    private EVanState vanState = EVanState.Entering;
    private Vector3 vanDestination;
    
    private VanSpawnPoint spawnPointObject;
    

    private void Awake()
    {
        Init();
        active = false;
        boxCollider = GetComponent<BoxCollider2D>();
    }
    
    public Vector3 GetRandomSpawnLocation()
    {
        return transform.position + spawnLocations[Random.Range(0, spawnLocations.Count)].transform.localPosition;
    }

    private void Update()
    {
        // FOR DEBUGGING TODO: REMOVE
        //statusText.text = "Layer = " + spriteRenderer.sortingOrder + "\n" + (GetNumStates() - GetState()) + "\n";

        if (!active)
        {
            return;
        }
        
        spriteRenderer.sortingOrder = (-(int)transform.position.y - (int)(boxCollider.size.y));

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
        enabled = true;
    }
    
    public void Deactivate()
    {
        active = false;
        gameObject.SetActive(false);
        enabled = false;
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
        GameManager.Instance.enemyManager.OnVanDestroyed(this);
    }
}
