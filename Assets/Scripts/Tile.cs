using System;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private List<Sprite> states;
    [SerializeField] private int initialState;
    [SerializeField] private float timeBetweenStates = 1.0f;
    
    private TileManager tileManager;
    
    private SpriteRenderer spriteRenderer;
    private int currentState;
    private float changeStateTime;

    private void Awake()
    {
        tileManager = GameObject.Find("TileManager").GetComponent<TileManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateState(initialState);
    }
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        int newState = currentState;
        
        if (collision.gameObject.CompareTag("Player"))
        {
            if (currentState == states.Count - 1)
            {
                // At max
                return;
            }

            newState++;
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            float visionAngle = Vector3.Angle(enemy.GetDirectionFacing(), transform.position - collision.gameObject.transform.position);
            //print(visionAngle);
            if (visionAngle > enemy.GetVisionAngle())
            {
                // Not viable
                return;
            }
            
            if (currentState == 0)
            {
                // At min
                return;
            }

            newState--;
        }

        var distanceFactor = Vector3.Distance(transform.position, collision.transform.position);
        distanceFactor = Mathf.Clamp(distanceFactor, 0.1f, 10.0f);
        distanceFactor /= 10.0f;

        if (!collision.gameObject.CompareTag("Projectile") && Time.time - changeStateTime < timeBetweenStates * distanceFactor)
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

    private void UpdateState(int newState)
    {
        if (newState < 0 || newState > states.Count - 1)
        {
            return;
        }
        
        int previousState = currentState;
        currentState = newState;
        spriteRenderer.sprite = states[currentState];

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
}
