using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ParticleManager : MonoBehaviour
{
    public enum EParticleType
    {
        Toxic,
        Spray,
        Explosion
    }
    
    [SerializeField] private List<Particles> particlesPrefabs;
    [SerializeField] private int PARTICLE_POOL_SIZE = 10;
    [SerializeField] private int MAX_PARTICLES = 10;
    private int particlesAlive = 0;
    private Dictionary<EParticleType, List<Particles>> particlePools = new Dictionary<EParticleType, List<Particles>>();
    private Dictionary<EParticleType, Particles> particlePrefabsDict = new Dictionary<EParticleType, Particles>();
    
    public void Init()
    {
        foreach (var particles in particlesPrefabs)
        {
            particlePrefabsDict.Add(particles.GetParticleType(), particles);
            
            EParticleType type = particles.GetParticleType();
            particlePools.Add(type, new List<Particles>());
            for (int j = 0; j < PARTICLE_POOL_SIZE; j++)
            {
                Particles newParticles = Instantiate(particles);
                newParticles.Deactivate();
                particlePools[type].Add(newParticles);
            }
        }
    }

    public Particles RequestParticlesAtPosition(EParticleType type,Vector3 position, float lifetime)
    {
        if (type == EParticleType.Toxic && particlesAlive >= MAX_PARTICLES)
        {
            return null;
        }
        
        Particles spawnedParticles = GetFirstAvailableParticlesFromPool(type);
        spawnedParticles.SetParticlesPosition(position);
        spawnedParticles.lifetime = lifetime;
        particlesAlive++;
        
        return spawnedParticles;
    }
    
    private Particles GetFirstAvailableParticlesFromPool(EParticleType type)
    {
        foreach (var particles in particlePools[type])
        {
            if (particles.active) 
                continue;
            
            // Found an inactive enemy
            particles.Activate();
            return particles;
        }
        
        // None active... make new one
        
        Particles newParticles = Instantiate(particlePrefabsDict[type], transform);
        newParticles.Activate();
        particlePools[type].Add(newParticles);
        return newParticles;
    }

    public void InformParticlesDestroyed()
    {
        particlesAlive--;
    }

    public void DestroyAllParticles()
    {
        foreach (var particles in particlePools[EParticleType.Toxic])
        {
            particles.Deactivate();
            particlesAlive--;
        }
        
        foreach (var particles in particlePools[EParticleType.Spray])
        {
            particles.Deactivate();
            particlesAlive--;
        }
    }
}
