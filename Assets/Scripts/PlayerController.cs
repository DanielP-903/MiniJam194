using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    public enum EMoveDirection
    {
        Left,
        Right,
        Up,
        Down
    }

    public static readonly Dictionary<EMoveDirection, Vector3> MoveDirectionLookup = new Dictionary<EMoveDirection, Vector3>()
    {
        {EMoveDirection.Up, Vector2.up},
        {EMoveDirection.Down, Vector2.down},
        {EMoveDirection.Left, Vector2.left},
        {EMoveDirection.Right, Vector2.right}   
    };

    private static readonly int MovementSpeed = Animator.StringToHash("MovementSpeed");
    private static readonly int Fire1 = Animator.StringToHash("Fire");
    private static readonly int Charging = Animator.StringToHash("Charging");

    [SerializeField] private float SPEED = 5;
    [SerializeField] private float TOXIC_RADIUS = 5;
    [SerializeField] private float PROJECTILE_CHARGE_TIME = 2;
    [SerializeField] private float POWER_SCALE = 10;
    [SerializeField] private float PROJECTILE_START_DAMAGE = 2;
    [SerializeField] private float FIRE_RATE = 1;
    [SerializeField] private float LOBBER_ARROW_OFFSET = 5.0f;
    
    [SerializeField] private int PROJECTILE_POOL_SIZE = 5;
    [SerializeField] private Projectile projectilePrefab;
    
    [SerializeField] private AudioClip AUDIO_CLIP_MOVE;
    [SerializeField] private AudioClip AUDIO_CLIP_CHARGE;
    [SerializeField] private AudioClip AUDIO_CLIP_FIRE;

    [SerializeField] private GameObject lobberArrow;
    [SerializeField] private TMP_Text statusText;
    

    private const float MAX_HEALTH = 100.0f;
    private float currentHealth = MAX_HEALTH;
    private float projectileStartTime;
    
    private readonly List<Projectile> projectilePool = new List<Projectile>();
    
    private bool moveUp;
    private bool moveDown;
    private bool moveLeft;
    private bool moveRight;
    private Rigidbody2D rb;
    private Animator animator;
    private CircleCollider2D circleCollider;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    private float lastPlayedMoveAudioTime = 0.0f;
    
    private bool canFire = true;
    private bool isCharging = false;
    private float lastFireTime;

    private bool hasPlayedDeathAnim = false;
    
    private EMoveDirection facingDirection = EMoveDirection.Up;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        circleCollider = gameObject.AddComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        circleCollider.radius = TOXIC_RADIUS;
        circleCollider.isTrigger = true;
        lobberArrow.SetActive(false);

        for (int i = 0; i < PROJECTILE_POOL_SIZE; i++)
        {
            Projectile newProjectile = Instantiate(projectilePrefab);
            newProjectile.Deactivate();
            projectilePool.Add(newProjectile);
        }
    }

    // Update is called once per frame
    private void Update()
    {        
        if (GameManager.Instance.isGameOver)
        {
            rb.linearVelocity = Vector3.zero;
            audioSource.Stop();

            if (!GameManager.Instance.hasWon && !hasPlayedDeathAnim)
            {
                // you died X_X
                animator.Play("PlayerDeathForwards");
                hasPlayedDeathAnim = true;
            }
            
            return;
        }

        ProcessInput();

        spriteRenderer.sortingOrder = (-(int)transform.position.y);

        if (rb.linearVelocity.x > 0 || facingDirection == EMoveDirection.Right)
        {
            spriteRenderer.flipX = true;
        }
        else if (rb.linearVelocity.x < 0 || facingDirection == EMoveDirection.Left)
        {
            spriteRenderer.flipX = false;
        }
        
        animator.SetFloat(MovementSpeed, rb.linearVelocity.magnitude);

        if (rb.linearVelocity.magnitude > 0.2f && Time.time - lastPlayedMoveAudioTime > 0.5f)
        {
            audioSource.PlayOneShot(AUDIO_CLIP_MOVE, Random.Range(0.5f, 0.7f));
            lastPlayedMoveAudioTime =  Time.time;
        }

        float scaleValue = ((float)GameManager.Instance.tileManager.GetNumberOfTilesCaptured() / GameManager.Instance.tileManager.GetNumberOfTiles()); 
        float scaleAmount = Mathf.Lerp(1.0f, 2.5f, scaleValue);
        transform.localScale = new Vector3(1.0f + scaleAmount, 1.0f + scaleAmount, 1.0f);
        circleCollider.radius = TOXIC_RADIUS + (scaleAmount - 1.0f);
        
        if (isCharging)
        {
            if (Time.time - projectileStartTime > PROJECTILE_CHARGE_TIME)
            {
                LobProjectile();
            }
        }
        else
        {
            if (Time.time - lastFireTime > FIRE_RATE)
            {
                canFire = true;
            }
        }
    }

    private void ProcessInput()
    {
        Vector2 velocity = Vector2.zero;
        if (moveUp)
        {
            velocity += Vector2.up;
            changeFacingDirection(EMoveDirection.Up);
        }
        if (moveDown)
        {
            velocity += Vector2.down;
            changeFacingDirection(EMoveDirection.Down);
        }
        if (moveRight)
        {
            velocity += Vector2.right;
            changeFacingDirection(EMoveDirection.Right);
        }
        if (moveLeft)
        {
            velocity += Vector2.left;
            changeFacingDirection(EMoveDirection.Left);
        }
        
        if (isCharging)
        {
            lobberArrow.transform.localPosition =  
                MoveDirectionLookup[facingDirection] * (LOBBER_ARROW_OFFSET + ((Time.time - projectileStartTime) / PROJECTILE_CHARGE_TIME));
            return;
        }
        
        rb.linearVelocity = velocity.normalized * SPEED;
    }

    private void changeFacingDirection(EMoveDirection direction)
    {
        facingDirection = direction;

        float rotationAngle = 0.0f;
        switch (facingDirection)
        {
            case EMoveDirection.Left:
                rotationAngle = 90.0f;
                break;
            case EMoveDirection.Right:
                rotationAngle = -90.0f;
                break;
            case EMoveDirection.Up:
                rotationAngle = 0.0f;
                break;
            case EMoveDirection.Down:
                rotationAngle = 180.0f;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        lobberArrow.transform.rotation = Quaternion.Euler(0,0,rotationAngle);
    }
    
    public void Move(InputAction.CallbackContext context)
    {        
        if (GameManager.Instance.isGameOver)
        {
            return;
        }

        Vector2 value = context.ReadValue<Vector2>();

        moveRight = value.x > 0.5f;
        moveLeft = value.x < -0.5f;
        moveUp = value.y > 0.5f;
        moveDown = value.y < -0.5f;
    }

    public void Fire(InputAction.CallbackContext context)
    {        
        if (GameManager.Instance.isGameOver)
        {
            return;
        }

        if (context.performed)
        {        
            audioSource.PlayOneShot(AUDIO_CLIP_CHARGE, 0.8f);
            animator.Play("PlayerChargeSide");
            isCharging = true;
            lobberArrow.SetActive(true);
            rb.linearVelocity = Vector2.zero;
            projectileStartTime = Time.time;
        }
        else if (context.canceled && isCharging)
        {
            LobProjectile();
        }
    }

    private void LobProjectile()
    {       
        isCharging = false;
        lobberArrow.SetActive(false);
        if (!canFire)
        {
            return;
        }
        canFire = false;
        lastFireTime = Time.time;
        
        audioSource.PlayOneShot(AUDIO_CLIP_FIRE, 0.8f);
        
        float power = (Time.time - projectileStartTime) / PROJECTILE_CHARGE_TIME;
        power = Mathf.Clamp(power, 0.2f, 1.0f);
        power *= POWER_SCALE;
            
        Vector3 position = transform.position + MoveDirectionLookup[facingDirection];
        Projectile newProjectile = GetFirstAvailableProjectileFromPool();
        newProjectile.transform.position = position;
        newProjectile.Fire(position + (MoveDirectionLookup[facingDirection] * power), power / 2.0f);
        animator.Play("PlayerFireSide");
    
        //print("Firing with power: " + power);
    }
    
    private Projectile GetFirstAvailableProjectileFromPool()
    {
        foreach (var projectile in projectilePool)
        {
            if (projectile.active) 
                continue;
            
            // Found an inactive enemy
            projectile.Activate();
            return projectile;
        }
        
        // None active... make new one
        
        Projectile newProjectile = Instantiate(projectilePrefab, transform);
        newProjectile.Activate();
        projectilePool.Add(newProjectile);
        return newProjectile;
    }

    
    // private void OnGUI()
    // {
    //     GUIStyle style = new GUIStyle();
    //     style.fontSize = 50;
    //
    //     float progress = ((Time.time - projectileStartTime) / PROJECTILE_CHARGE_TIME) * 100;
    //     progress = Mathf.Clamp(progress, 0, 100);
    //
    //     if (!isCharging)
    //     {
    //         progress = 0;
    //     }
    //     GUI.Label(new Rect(10,70,200,40), "Charged " + progress.ToString("#0.0") + "%", style);
    // }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Enemy") || collision is not CircleCollider2D)
            return;
        
        Enemy collidedEnemy = collision.gameObject.GetComponent<Enemy>();
        
        // Detected enemy spray
        if (!collidedEnemy.GetSprayingInLocation(transform.position)) 
            return;
        
        // We are inside the spray location;
        currentHealth -= collidedEnemy.GetPlayerDamageTickAmount() * Time.deltaTime;
        if (currentHealth <= 0)
        {
            // We are dead.
            GameManager.Instance.OnPlayerDeath();
            currentHealth = 0;
        }
        
        statusText.text = currentHealth.ToString("#0");
    }

    public float GetCurrentProjectileDamage()
    {
        return PROJECTILE_START_DAMAGE * GameManager.Instance.GetScoreMultiplier();
    }
}
