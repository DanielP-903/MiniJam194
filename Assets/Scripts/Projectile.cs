using UnityEngine;

public class Projectile : MonoBehaviour
{
    private CircleCollider2D circleCollider;
    
    private bool isFiring;
    private Vector3 savedStartPos;
    private Vector3 savedEndPos;
    
    [SerializeField] private float BASE_SPEED = 10;
    private float speed = 10;
    private float elapsedTime;
    
    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        circleCollider.enabled = false;
    }

    private void Update()
    {
        if (!isFiring)
        {
            return;
        }
        
        elapsedTime += Time.deltaTime;
        float alpha = elapsedTime * speed;
        alpha = Mathf.Clamp01(alpha);
        transform.position = Vector3.Lerp(savedStartPos, savedEndPos, alpha);
        float sin = 1 + Mathf.Sin(Mathf.Lerp(0, Mathf.PI, alpha));
        transform.localScale = new Vector3(sin, sin, 1);
        //print(sin);

        // Have we finished?
        if (alpha > 0.999f)
        {
            transform.position = savedEndPos;
            transform.localScale = new Vector3(1, 1, 1);
            circleCollider.enabled = true;
            isFiring = false;
            OnImpact();
        }
    }
    
    public void Fire(Vector3 endPos)
    {
        isFiring = true;
        elapsedTime = 0.0f;
        savedStartPos = transform.position;
        savedEndPos = endPos;
        speed = BASE_SPEED / Vector3.Distance(savedStartPos, savedEndPos);
    }

    private void OnImpact()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, circleCollider.radius);
        foreach(var c in colliders) 
        {
            c.gameObject.SendMessage("OnTriggerStay2D", circleCollider);
        }
        
        //print("Impacted!");
        Destroy(gameObject);
    }
}
