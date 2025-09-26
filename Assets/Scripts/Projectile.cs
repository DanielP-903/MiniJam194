using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Fire(float power, Vector2 direction)
    {
        rb.AddForce(direction.normalized * power);
    }
}
