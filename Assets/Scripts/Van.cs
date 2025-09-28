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
    [SerializeField] private AnimationClip VAN_DESTROY_ANIM;
    [SerializeField] private AudioClip AUDIO_CLIP_VAN_MOVING;
    [SerializeField] private AudioClip AUDIO_CLIP_VAN_DESTROYED;

    private AudioSource audioSource;
    private BoxCollider2D boxCollider;
    
    private EVanState vanState = EVanState.Entering;
    private Vector3 vanDestination;
    private float vanDeathDuration;
    private float deathStartTime;
    
    private VanSpawnPoint spawnPointObject;
    

    private void Awake()
    {
        Init();
        active = false;
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        
        vanDeathDuration = VAN_DESTROY_ANIM.length;
    }
    
    public Vector3 GetRandomSpawnLocation()
    {
        return transform.position + spawnLocations[Random.Range(0, spawnLocations.Count)].transform.localPosition;
    }

    private void Update()
    {
        if (GameManager.Instance.isGameOver)
        {
            audioSource.Stop();
            return;
        }
        
        //print("I AM PLAYING: " + audioSource.clip.name);
        
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
                    audioSource.Stop();
                }
                break;
            case EVanState.Idle:
                audioSource.Stop();
                break;
            case EVanState.Dead:
                if (Time.time - deathStartTime > vanDeathDuration)
                {
                    // Ok we've suffered enough, kill me
                    Deactivate();
                }
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
        gameObject.SetActive(true);
        vanState = EVanState.Entering;
        audioSource.enabled = true;
        audioSource.Play();
        UpdateState(initialState);
        enabled = true;
    }
    
    public void Deactivate()
    {
        active = false;
        gameObject.SetActive(false);
        audioSource.enabled = false;
        audioSource.clip = AUDIO_CLIP_VAN_MOVING;
        audioSource.loop = true;
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
        deathStartTime =  Time.time;
        animator.enabled = true;
        animator.Play("VanDeath");

        audioSource.clip = AUDIO_CLIP_VAN_DESTROYED;
        audioSource.loop = false;
        audioSource.Play();
        
        GameManager.Instance.enemyManager.OnVanDestroyed(this);
        GameManager.Instance.IncreaseScore(BONUS_SCORE_REWARD);
    }
}
