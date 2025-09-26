using System;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private List<Sprite> states;
    [SerializeField] private int initialState = 0;
    [SerializeField] private float timeBetweenStates = 1.0f;
    
    private TileManager tileManager;
    
    private SpriteRenderer spriteRenderer;
    private int currentState = 0;
    private float changeStateTime = 0.0f;

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
            if (currentState == 0)
            {
                // At min
                return;
            }

            newState--;
        }
        
        float distanceFactor = 1.0f;
        distanceFactor = Vector3.Distance(transform.position, collision.transform.position);
        distanceFactor = Mathf.Clamp(distanceFactor, 0.1f, 10.0f);
        distanceFactor /= 10.0f;

        if (Time.time - changeStateTime < timeBetweenStates * distanceFactor)
        {
            return;
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
}
