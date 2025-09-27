using System;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    private static readonly int MovementVelocity = Animator.StringToHash("MovementVelocity");
    private static readonly int IsSpraying = Animator.StringToHash("IsSpraying");
    
    private enum EEnemyState
    {
        Moving,
        Spraying
    }

    private enum EEnemyPersonality
    {
        Grunt,  // Moves randomly
        Bold,   // Moves towards the player
        Aggressive  // Moves directly to the player
    }

    [SerializeField] private float ENEMY_RADIUS = 5.0f;
    [SerializeField] private float ENEMY_VISION_ANGLE = 45.0f;
    [SerializeField] private float ENEMY_BASE_MOVEMENT_SPEED = 2.0f;
    [SerializeField] private float ENEMY_MIN_MOVE_TIME = 2.0f;
    [SerializeField] private float ENEMY_MAX_MOVE_TIME = 5.0f;
    [SerializeField] private float ENEMY_SPRAY_MIN_DURATION = 2.0f;
    [SerializeField] private float ENEMY_SPRAY_MAX_DURATION = 3.0f;
    [SerializeField] private int ENEMY_KILL_BONUS_SCORE = 100;
    [SerializeField] private TMP_Text statusText;

    private const float ENEMY_PLAYER_DAMAGE_TICK = 2.0f;

    private const float ENEMY_MAX_HEALTH = 100.0f;
    private float currentHealth = ENEMY_MAX_HEALTH;
    private float enemyMovementSpeed = 2.0f;
    
    private PlayerController.EMoveDirection myMoveDirection = PlayerController.EMoveDirection.Down;

    private EEnemyPersonality personality = EEnemyPersonality.Grunt;
    
    private EnemyManager enemyManager;
    private GameManager gameManager;
    private PlayerController playerController;
    private TileManager tileManager;
    
    private Animator animator;
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
    
    private bool active;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        // This is in inefficient TODO: REFACTOR!
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        tileManager = GameObject.Find("TileManager").GetComponent<TileManager>();
        enemyManager = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        circleCollider = gameObject.AddComponent<CircleCollider2D>();
        circleCollider.radius = ENEMY_RADIUS;
        circleCollider.isTrigger = true;

        currentHealth = ENEMY_MAX_HEALTH;
    }

    private void Update()
    {        
        if (!active)
        {
            return;
        }
        
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
        if (!active)
        {
            return;
        }
        
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
                    rb.linearVelocity = directionTo * enemyMovementSpeed;
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
        
        statusText.text = currentHealth.ToString("#0") + "\n" + personality.ToString() + " | " + currentState.ToString() + " : " + (Time.time - stateStartTime).ToString("#0.00");
    }

    private void initState()
    {
        stateStartTime = Time.time;

        switch (currentState)
        {
            case EEnemyState.Moving:
                // Tell the enemy where it's going next
                circleCollider.enabled = false;

                switch (personality)
                {
                    case EEnemyPersonality.Grunt:
                        destination = tileManager.GetRandomTile().transform.position;
                        directionTo = (destination - transform.position).normalized;
                        break;
                    case EEnemyPersonality.Bold:
                    case EEnemyPersonality.Aggressive:
                    //case EEnemyPersonality.Scared:
                        float radius = personality == EEnemyPersonality.Aggressive ? 2.0f : playerController.GetComponent<CircleCollider2D>().radius + 1.0f;
                        destination = tileManager.GetRandomTileInRadius(playerController.transform.position, radius).transform.position;
                        directionTo = (destination - transform.position).normalized;
                        break;
                    
                }

                enemyMovementSpeed = personality == EEnemyPersonality.Aggressive
                    ? ENEMY_BASE_MOVEMENT_SPEED + 3
                    : ENEMY_BASE_MOVEMENT_SPEED; 
                
                // if (personality == EEnemyPersonality.Scared)
                // {
                //     // Go the other way
                //     directionTo *= -1;
                // }
                
                movementTimer = Random.Range(ENEMY_MIN_MOVE_TIME, ENEMY_MAX_MOVE_TIME);
                
                // play moving animation
                animator.SetFloat(MovementVelocity, enemyMovementSpeed);
                animator.SetBool(IsSpraying, false);

                break;
            
            case EEnemyState.Spraying:
                // Allow enemy to spray
                circleCollider.enabled = true;
                sprayingTimer = Random.Range(ENEMY_SPRAY_MIN_DURATION, ENEMY_SPRAY_MAX_DURATION);
                
                // play spraying animation
                animator.SetFloat(MovementVelocity, 0.0f);
                animator.SetBool(IsSpraying, true);

                break;
        }
        
        statusText.text = currentState.ToString() + " : 0.0";
    }
    
    private void changeState(EEnemyState newState)
    {
        currentState = newState;
        initState();
    }

    private Vector3 GetDirectionFacing()
    {
        return PlayerController.MoveDirectionLookup[myMoveDirection];
    }

    public void Activate()
    {
        active = true;
        personality = (EEnemyPersonality)Random.Range(0, Enum.GetValues(typeof(EEnemyPersonality)).Length);
        print("I am " + personality);
        changeState(EEnemyState.Moving);
        myMoveDirection = PlayerController.EMoveDirection.Down;
        currentHealth = ENEMY_MAX_HEALTH;
        
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

    public bool GetSprayingInLocation(Vector3 position)
    {
        return Vector3.Angle(GetDirectionFacing(), transform.position - position) > ENEMY_VISION_ANGLE;
    }

    public float GetPlayerDamageTickAmount()
    {
        return ENEMY_PLAYER_DAMAGE_TICK;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Projectile"))
        {
            // Uh oh, I've been hit!
            currentHealth -= playerController.GetCurrentProjectileDamage();
            if (currentHealth <= 0)
            {
                gameManager.IncreaseScore(ENEMY_KILL_BONUS_SCORE);
                enemyManager.OnEnemyDestroyed();
                Deactivate();
            }
        }
    }
}
