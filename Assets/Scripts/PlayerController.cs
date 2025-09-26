using UnityEngine;
using UnityEngine.InputSystem;

public class BasicTopDownPlayerController : MonoBehaviour
{
    [SerializeField] private float SPEED = 5;
    [SerializeField] private float TOXIC_RADIUS = 5;
    
    private bool moveUp = false;
    private bool moveDown = false;
    private bool moveLeft = false;
    private bool moveRight = false;
    private Rigidbody2D rb;
    private CircleCollider2D circleCollider;

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
    }

    private void ProcessInput()
    {
        Vector2 velocity = Vector2.zero;
        if (moveUp)
        {
            velocity += Vector2.up;
        }
        if (moveDown)
        {
            velocity += Vector2.down;
        }

        if (moveRight)
        {
            velocity += Vector2.right;
        }
        if (moveLeft)
        {
            velocity += Vector2.left;
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
    
}
