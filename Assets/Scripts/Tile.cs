using System;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private List<Color> states;
    [SerializeField] private int initialState = 0;
    [SerializeField] private float timeBetweenStates = 1.0f;
    
    private SpriteRenderer spriteRenderer;
    private int currentState = 0;
    private float triggerEnterTime = 0.0f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateState(initialState);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        triggerEnterTime = Time.time;
    }
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        print("Colliding with " + collision.name);

        if (Time.time - triggerEnterTime > timeBetweenStates)
        {
            triggerEnterTime = Time.time;
            UpdateState(currentState + 1);
        }
    }

    private void UpdateState(int newState)
    {
        if (newState < 0 || newState > states.Count - 1)
        {
            return;
        }
        
        currentState = newState;
        spriteRenderer.color = states[currentState];
    }
}
