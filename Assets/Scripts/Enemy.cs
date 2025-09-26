using System;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    public enum EEnemyState
    {
        Moving,
        Spraying
    }
    
    [SerializeField] private float ENEMY_RADIUS = 5.0f;
    [SerializeField] private float ENEMY_VISION_ANGLE = 45.0f;
    [SerializeField] private float ENEMY_MOVEMENT_SPEED = 5.0f;
    [SerializeField] private float ENEMY_MIN_MOVE_TIME = 2.0f;
    [SerializeField] private float ENEMY_MAX_MOVE_TIME = 5.0f;
    [SerializeField] private float ENEMY_SPRAY_MIN_DURATION = 2.0f;
    [SerializeField] private float ENEMY_SPRAY_MAX_DURATION = 3.0f;
    [SerializeField] private TMP_Text statusText;

    private static PlayerController.EMoveDirection myMoveDirection = PlayerController.EMoveDirection.Down;

    private PlayerController playerController;
    private TileManager tileManager;
    
    private Rigidbody2D rb;
    private CircleCollider2D circleCollider;
    private SpriteRenderer spriteRenderer;
    
    private EEnemyState currentState = EEnemyState.Moving;
    
    // TODO: if in full game, extract this stuff
    
    private float stateStartTime;

    // Moving State
    private Vector3 destination;
    private Vector2 directionTo;
    private float movementTimer;

    // Spraying State
    private float sprayingTimer;
    
    private Vector3 previousPosition;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        tileManager = GameObject.Find("TileManager").GetComponent<TileManager>();
        rb = GetComponent<Rigidbody2D>();
        circleCollider = gameObject.AddComponent<CircleCollider2D>();
        circleCollider.radius = ENEMY_RADIUS;
        circleCollider.isTrigger = true;
    }

    private void Update()
    {
        if (rb.linearVelocity.x > 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (rb.linearVelocity.x < 0)
        {
            spriteRenderer.flipX = false;
        }

        if (Mathf.Abs(rb.linearVelocity.x) > Mathf.Abs(rb.linearVelocity.y))
        {
            if (rb.linearVelocity.x > 0)
            {
                myMoveDirection = PlayerController.EMoveDirection.Right;
            }
            else if (rb.linearVelocity.x < 0)
            {
                myMoveDirection = PlayerController.EMoveDirection.Left;
            }
        }
        else
        {
            if (rb.linearVelocity.y > 0)
            {
                myMoveDirection = PlayerController.EMoveDirection.Down;
            }
            else if (rb.linearVelocity.y < 0)
            {
                myMoveDirection = PlayerController.EMoveDirection.Up;
            }
        }
    }

    
    // Fixed update is called once per frame
    private void FixedUpdate()
    {
        tickState();
        
        previousPosition = transform.position;
    }

    private void tickState()
    {
        switch (currentState)
        {
            case EEnemyState.Moving:
                if (Time.time - stateStartTime > movementTimer || 
                    Vector3.Distance(transform.position, destination) < 0.5f ||
                    (Time.time - stateStartTime > 0.2f && transform.position == previousPosition))
                {
                    // Stop moving, I got distracted/bored/walked-into-a-wall/etc
                    changeState(EEnemyState.Spraying);
                    rb.linearVelocity = Vector2.zero;
                }
                else
                {
                    rb.linearVelocity = directionTo * ENEMY_MOVEMENT_SPEED;
                    Debug.DrawLine(transform.position, destination, Color.red);
                }
                break;
            case EEnemyState.Spraying:
                rb.linearVelocity = Vector2.zero;
                if (Time.time - stateStartTime > sprayingTimer)
                {
                    // Alright, we're done here, move along
                    changeState(EEnemyState.Moving);
                }
                break;
        }
        
        statusText.text = currentState.ToString() + " : " + (Time.time - stateStartTime).ToString("#0.00");
    }

    private void initState()
    {
        stateStartTime = Time.time;

        switch (currentState)
        {
            case EEnemyState.Moving:
                // Tell the enemy where it's going next
                circleCollider.enabled = false;
                destination = tileManager.GetRandomTileInRadius(playerController.transform.position, playerController.GetComponent<CircleCollider2D>().radius + 1.0f).transform.position;//tileManager.GetRandomTile().transform.position;
                directionTo = (destination - transform.position).normalized;
                movementTimer = Random.Range(ENEMY_MIN_MOVE_TIME, ENEMY_MAX_MOVE_TIME);
                
                // play moving animation
                // TODO

                break;
            
            case EEnemyState.Spraying:
                // Allow enemy to spray
                circleCollider.enabled = true;
                sprayingTimer = Random.Range(ENEMY_SPRAY_MIN_DURATION, ENEMY_SPRAY_MAX_DURATION);
                
                // play spraying animation
                // TODO

                break;
        }
        
        statusText.text = currentState.ToString() + " : 0.0";
    }
    
    private void changeState(EEnemyState newState)
    {
        if (newState == currentState)
        {
            return;
        }
        
        currentState = newState;
        initState();
    }
    
    public float GetVisionAngle()
    {
        return ENEMY_VISION_ANGLE;
    }

    public Vector3 GetDirectionFacing()
    {
        return PlayerController.MoveDirectionLookup[myMoveDirection];
    }
}
