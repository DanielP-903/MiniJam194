using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private ParticleSystem ps;
    private CircleCollider2D circleCollider;
    
    private bool isFiring;
    private Vector3 savedStartPos;
    private Vector3 savedEndPos;
    private float savedPower;
    
    [SerializeField] private float BASE_SPEED = 10;
    private float speed = 10;
    private float elapsedTime;

    private float initialScale = 1.0f;
    
    public bool active { get; private set; }

    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        if (!active)
        {
            return;
        }
        
        if (!isFiring)
        {
            return;
        }
        
        elapsedTime += Time.deltaTime;
        float alpha = elapsedTime * speed;
        alpha = Mathf.Clamp01(alpha);
        transform.position = Vector3.Lerp(savedStartPos, savedEndPos, alpha);
        float sin = initialScale + Mathf.Sin(Mathf.Lerp(0, Mathf.PI, alpha));
        transform.localScale = new Vector3(sin, sin, 1);
        //print(sin);

        // Have we finished?
        if (alpha > 0.999f)
        {
            transform.position = savedEndPos;
            transform.localScale = new Vector3(1, 1, 1);
            circleCollider.enabled = true;
            circleCollider.radius = savedPower;
            isFiring = false;
            OnImpact();
        }
    }
    
    public void Fire(Vector3 endPos, float power)
    {
        isFiring = true;
        elapsedTime = 0.0f;
        savedStartPos = transform.position;
        savedEndPos = endPos;
        savedPower = Mathf.Clamp(power, 1.5f,5.0f);
        print("Power = " + power);
        initialScale = savedPower/ 5.0f;
        speed = BASE_SPEED / Vector3.Distance(savedStartPos, savedEndPos);
    }

    private void OnImpact()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, circleCollider.radius);
        foreach(var c in colliders) 
        {
            c.gameObject.SendMessage("OnTriggerStay2D", circleCollider, SendMessageOptions.DontRequireReceiver);
        }
        
        //print("Impacted!");
        Deactivate();
    }

    public float GetPower()
    {
        return savedPower;
    }

    
    public void Activate()
    {
        active = true;
        gameObject.SetActive(true);
        ps.Play();
        circleCollider.enabled = false;
        enabled = true;
    }
    
    public void Deactivate()
    {
        active = false;
        gameObject.SetActive(false);
        ps.Stop();
        enabled = false;
    }
}
