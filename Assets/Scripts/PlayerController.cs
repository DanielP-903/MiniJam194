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
    [SerializeField] private float PROJECTILE_MAX_POWER = 10;
    [SerializeField] private float PROJECTILE_MIN_POWER = 2;
    [SerializeField] private float PROJECTILE_START_DAMAGE = 2;
    [SerializeField] private float FIRE_RATE = 1;
    [SerializeField] private Projectile projectile;
    [SerializeField] private GameManager gameManager;

    private const float MAX_HEALTH = 100.0f;
    private float currentHealth = MAX_HEALTH;
    
    private const float POWER_SCALE = 20;

    private float projectileStartTime;
    
    private bool moveUp;
    private bool moveDown;
    private bool moveLeft;
    private bool moveRight;
    private Rigidbody2D rb;
    private CircleCollider2D circleCollider;
    private float savedPowerPercent;

    private bool canFire = true;
    private bool isCharging = false;
    private float lastFireTime;
    
    private EMoveDirection facingDirection = EMoveDirection.Up;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        circleCollider = gameObject.AddComponent<CircleCollider2D>();
        circleCollider.radius = TOXIC_RADIUS;
        circleCollider.isTrigger = true;
    }

    // Update is called once per frame
    private void Update()
    {
        ProcessInput();

        if (Time.time - lastFireTime > FIRE_RATE)
        {
            canFire = true;
        }
    }

    private void ProcessInput()
    {
        if (isCharging)
        {
            return;
        }
        
        Vector2 velocity = Vector2.zero;
        if (moveUp)
        {
            velocity += Vector2.up;
            facingDirection = EMoveDirection.Up;
        }
        if (moveDown)
        {
            velocity += Vector2.down;
            facingDirection = EMoveDirection.Down;
        }
        if (moveRight)
        {
            velocity += Vector2.right;
            facingDirection = EMoveDirection.Right;
        }
        if (moveLeft)
        {
            velocity += Vector2.left;
            facingDirection = EMoveDirection.Left;
        }
        
        rb.linearVelocity = velocity.normalized * SPEED;
    }

    public void Move(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();

        moveRight = value.x > 0.5f;
        moveLeft = value.x < -0.5f;
        moveUp = value.y > 0.5f;
        moveDown = value.y < -0.5f;

        // Print current action bools
        //Debug.Log("Move Input (left, right, up, down): " + moveLeft + ", " + moveRight+ ", " + moveUp + ", " + moveDown);
    }

    public void Fire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isCharging = true;
            rb.linearVelocity = Vector2.zero;
            projectileStartTime = Time.time;
            //print("Charging...");
        }
        else if (context.canceled)
        {
            isCharging = false;
            if (!canFire)
            {
                return;
            }
            canFire = false;
            lastFireTime = Time.time;
            
            float power = Time.time - projectileStartTime;
            power *= POWER_SCALE;
            power = Mathf.Clamp(power, PROJECTILE_MIN_POWER, PROJECTILE_MAX_POWER);
            savedPowerPercent = (power / PROJECTILE_MAX_POWER) * 100;   // TODO: will be useful for the shader 0w0
            
            Vector3 position = transform.position + MoveDirectionLookup[facingDirection];
            Projectile newProjectile = Instantiate(projectile, position, Quaternion.identity);
            newProjectile.Fire(position + (MoveDirectionLookup[facingDirection] * power));
            
           // print("Firing with power: " + power);
        }
    }
    private void OnGUI()
    {
        //GUIStyle style = new GUIStyle();
        //style.fontSize = 50;
        //GUI.Label(new Rect(10,70,200,40), "Charged " + savedPowerPercent + "%", style);
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
        if (collidedEnemy.GetSprayingInLocation(transform.position))
        {
            // We are inside the spray location;
            currentHealth -= collidedEnemy.GetPlayerDamageTickAmount() * Time.deltaTime;
            if (currentHealth <= 0)
            {
                // We are dead.
                gameManager.OnPlayerDeath();
            }
        }
    }

    public float GetCurrentProjectileDamage()
    {
        return PROJECTILE_START_DAMAGE * gameManager.GetScoreMultiplier();
    }
}
