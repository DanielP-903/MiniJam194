using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private List<Sprite> states;
    [SerializeField] protected int initialState;
    [SerializeField] private bool canBeHealed = true;
    [SerializeField, Tooltip("AKA enemy sprays on tile")] private float timeBetweenNegativeStates = 2.0f;    // AKA enemy sprays on tile
    [SerializeField, Tooltip("AKA player goes over tile")] private float timeBetweenPositiveStates = 1.0f;    // AKA player goes over tile
    
    private TileManager tileManager;
    
    protected SpriteRenderer spriteRenderer;
    private int currentState;
    private float changeStateTime;
    
    protected bool active = true;

    private void Awake()
    {
        Init();
    }

    protected void Init()
    {
        tileManager = GameObject.Find("TileManager").GetComponent<TileManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateState(initialState);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!active)
        {
            return;
        }
        
        int newState = currentState;

        float timeDelay;
        
        if (collision.gameObject.CompareTag("Player"))
        {
            if (currentState == states.Count - 1)
            {
                // At max
                return;
            }

            timeDelay = timeBetweenPositiveStates;
            newState++;
        }
        else if (collision.gameObject.CompareTag("Enemy") && collision is CircleCollider2D)
        {
            if (!canBeHealed)
            {
                return;
            }
            
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (!enemy.GetSprayingInLocation(transform.position))
            {
                return;
            }
            
            if (currentState == 0)
            {
                // At min
                return;
            }

            timeDelay = timeBetweenNegativeStates;
            newState--;
        }
        else
        {
            timeDelay = timeBetweenPositiveStates;
        }

        var distanceFactor = Vector3.Distance(transform.position, collision.transform.position);
        distanceFactor = Mathf.Clamp(distanceFactor, 0.1f, 10.0f);
        distanceFactor /= 10.0f;

        if (!collision.gameObject.CompareTag("Projectile") && Time.time - changeStateTime < timeDelay * distanceFactor)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Projectile"))
        {
            if (currentState == states.Count - 1)
            {
                // At max
                return;
            }

            if (distanceFactor < 0.2f)
            {
                newState++;
                if (distanceFactor < 0.1f)
                {
                    newState++;
                    if (distanceFactor < 0.05f)
                    {
                        newState++;
                    }
                }
            }
            newState = Mathf.Clamp(newState, 0, states.Count - 1);
        }
        
        changeStateTime = Time.time;
        UpdateState(newState);
    }

    protected void UpdateState(int newState)
    {        
        if (!active)
        {
            return;
        }
        
        if (newState < 0 || newState > states.Count - 1)
        {
            return;
        }
        
        int previousState = currentState;
        currentState = newState;
        spriteRenderer.sprite = states[currentState];
        transform.GetChild(0).TryGetComponent(out SpriteRenderer childSpriteRenderer);
        if (childSpriteRenderer)
        {
            childSpriteRenderer.color = new Color(0.0f, 0.0f, 0.0f, (float)(currentState + 1) / states.Count);
        }

        if (newState == states.Count - 1)
        {
            // Reached max state
            OnReachedMaxState();
        }
        
        tileManager.InformStateChange(this, previousState);
    }

    public int GetState()
    {
        return currentState;
    }

    public bool IsAtMaxState()
    {
        return currentState == states.Count - 1;
    }
    
    protected virtual void OnReachedMaxState()
    {
        // does something...
    }
}
