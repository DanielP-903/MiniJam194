using UnityEngine;
using UnityEngine.InputSystem;

public class BasicTopDownPlayerController : MonoBehaviour
{
    [SerializeField] public float SPEED = 5;
    
    private bool moveUp = false;
    private bool moveDown = false;
    private bool moveLeft = false;
    private bool moveRight = false;
    private Rigidbody2D rigidbody2D;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        Debug.Log("Hello World!");
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
        
        rigidbody2D.linearVelocity = velocity.normalized * SPEED;
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
        Debug.Log("Move Input (left, right, up, down): " + moveLeft + ", " + moveRight+ ", " + moveUp + ", " + moveDown);
    }
    
    // TODO: for reference only
    
    // Input Actions
    // W
    public void Up(InputAction.CallbackContext context)
    {
        float value = context.ReadValue<float>();
        moveUp = value > 0;
        Debug.Log("Forward detected");
    }
    // S
    public void Down(InputAction.CallbackContext context)
    {
        float value = context.ReadValue<float>();
        moveDown = value > 0;
        Debug.Log("Backward detected");
    }
    // A
    public void Left(InputAction.CallbackContext context)
    {
        float value = context.ReadValue<float>();
        moveLeft = value > 0;
        Debug.Log("Left detected");
    }
    // D
    public void Right(InputAction.CallbackContext context)
    {
        float value = context.ReadValue<float>();
        moveRight = value > 0;
        Debug.Log("Right detected");
    }
}
