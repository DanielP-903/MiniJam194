using System;
using UnityEngine;

public class Particles : MonoBehaviour
{
    private ParticleSystem ps;
    public bool active {get; private set;}
    public float lifetime {get; set;}

    private float startTime;
    [SerializeField] private ParticleManager.EParticleType particleType;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (Time.time - startTime > lifetime)
        {
            // That's us done!
            Deactivate();
            GameManager.Instance.particleManager.InformParticlesDestroyed();
        }
    }

    public void SetParticlesPosition(Vector3 position)
    {
        transform.position = position;
    }
    
    public void Activate()
    {
        ps.Play(true);
        active = true;
        gameObject.SetActive(true);
        startTime = Time.time;
    }
    
    public void Deactivate()
    {
        ps.Play(false);
        active = false;
        gameObject.SetActive(false);
    }

    public ParticleManager.EParticleType GetParticleType()
    {
        return particleType;
    }
}
