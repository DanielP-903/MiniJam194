using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float ENEMY_RADIUS = 5.0f;
    [SerializeField] private float ENEMY_VISION_ANGLE = 45.0f;

    private const BasicTopDownPlayerController.EMoveDirection myMoveDirection = BasicTopDownPlayerController.EMoveDirection.Down;

    private Rigidbody2D rb;
    private CircleCollider2D circleCollider;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        circleCollider = gameObject.AddComponent<CircleCollider2D>();
        circleCollider.radius = ENEMY_RADIUS;
        circleCollider.isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float GetVisionAngle()
    {
        return ENEMY_VISION_ANGLE;
    }

    public Vector3 GetDirectionFacing()
    {
        return BasicTopDownPlayerController.MoveDirectionLookup[myMoveDirection];
    }
}
