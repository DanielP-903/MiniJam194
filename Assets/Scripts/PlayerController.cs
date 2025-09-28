using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    [SerializeField] private float SPEED = 5;
    [SerializeField] private float TOXIC_RADIUS = 5;
    [SerializeField] private float PROJECTILE_CHARGE_TIME = 2;
    [SerializeField] private float POWER_SCALE = 10;
    [SerializeField] private float PROJECTILE_START_DAMAGE = 2;
    [SerializeField] private float FIRE_RATE = 1;
    [SerializeField] private Projectile projectile;
    [SerializeField] private GameObject lobberArrow;

    private const float MAX_HEALTH = 100.0f;
    private float currentHealth = MAX_HEALTH;
    private float projectileStartTime;
    
    private bool moveUp;
    private bool moveDown;
    private bool moveLeft;
    private bool moveRight;
    private Rigidbody2D rb;
    private CircleCollider2D circleCollider;
    private SpriteRenderer spriteRenderer;
    private float savedPowerPercent;

    private bool canFire = true;
    private bool isCharging = false;
    private float lastFireTime;
    
    private EMoveDirection facingDirection = EMoveDirection.Up;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        circleCollider = gameObject.AddComponent<CircleCollider2D>();
        circleCollider.radius = TOXIC_RADIUS;
        circleCollider.isTrigger = true;
    }

    // Update is called once per frame
    private void Update()
    {
        ProcessInput();

        spriteRenderer.sortingOrder = (-(int)transform.position.y);

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
        Vector2 value = context.ReadValue<Vector2>();

        moveRight = value.x > 0.5f;
        moveLeft = value.x < -0.5f;
        moveUp = value.y > 0.5f;
        moveDown = value.y < -0.5f;
    }

    public void Fire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isCharging = true;
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
        if (!canFire)
        {
            return;
        }
        canFire = false;
        lastFireTime = Time.time;
            
        float power = (Time.time - projectileStartTime) / PROJECTILE_CHARGE_TIME;
        power = Mathf.Clamp(power, 0.2f, 1.0f);
        savedPowerPercent = power * 100;
        power *= POWER_SCALE;
            
        Vector3 position = transform.position + MoveDirectionLookup[facingDirection];
        Projectile newProjectile = Instantiate(projectile, position, Quaternion.identity);
        newProjectile.Fire(position + (MoveDirectionLookup[facingDirection] * power), power / 2.0f);
            
        //print("Firing with power: " + power);
    }
    
    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 50;

        float progress = ((Time.time - projectileStartTime) / PROJECTILE_CHARGE_TIME) * 100;
        progress = Mathf.Clamp(progress, 0, 100);

        if (!isCharging)
        {
            progress = 0;
        }
        GUI.Label(new Rect(10,70,200,40), "Charged " + progress.ToString("#0.0") + "%", style);
    }

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
        }
    }

    public float GetCurrentProjectileDamage()
    {
        return PROJECTILE_START_DAMAGE * GameManager.Instance.GetScoreMultiplier();
    }
}
