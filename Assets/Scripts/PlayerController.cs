using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BasicTopDownPlayerController : MonoBehaviour
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
    [SerializeField] private float FIRE_RATE = 1;
    [SerializeField] private Projectile projectile;

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
        //transform.Translate(velocity.normalized * (SPEED * Time.deltaTime), Space.World);
    }

    public void Move(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();

        moveRight = value.x > 0;
        moveLeft = value.x < 0;
        moveUp = value.y > 0;
        moveDown = value.y < 0;

        // Print current action bools
        //Debug.Log("Move Input (left, right, up, down): " + moveLeft + ", " + moveRight+ ", " + moveUp + ", " + moveDown);
    }

    public void Fire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            projectileStartTime = Time.time;
            //print("Charging...");
        }
        else if (context.canceled)
        {
            if (!canFire)
            {
                return;
            }
            canFire = false;
            lastFireTime = Time.time;
            
            float power = Time.time - projectileStartTime;
            power *= POWER_SCALE;
            power = Mathf.Clamp(power, PROJECTILE_MIN_POWER, PROJECTILE_MAX_POWER);
            savedPowerPercent = (power / PROJECTILE_MAX_POWER) * 100;
            
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
}
