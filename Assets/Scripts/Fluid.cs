using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Fluid : MonoBehaviour
{
    public List<Particle> Particles = new();

    public int maxParticles = 1000;
    public float gravity = -1.0f;
    public float stiffness = 0.008f;
    public float h = 3.0f;
    public int gridSize = 30;
    public int cellSize = 3;
    private SpatialHashTable hashTable;
    public float nearStiffness = 0.01f;
    public float restDensity = 10.0f;

    public float width = 10;
    public float height = 10;
    
    // Start is called before the first frame update
    private void Start()
    {
        hashTable = new SpatialHashTable(gridSize, cellSize);
        for (int x = 0; x < 10; x++)
        {
            for (int y = 1; y < 10; y++)
            {
                Particles.Add(new Particle(new Vector2(x, y), Vector2.zero));
            }
        }
    }

    private void Simulate()
    {
        // Pass 1
        foreach (var particle in Particles)
        {
            ApplyGravity(particle);
            AdvanceToPredictedPosition(particle);
            hashTable.Add(particle);
        }

        // Pass 2
        foreach (var particle in Particles)
        {
           DoubleDensityRelaxation(particle); 
        }

        // Pass 3
        foreach (var particle in Particles)
        {
            ResolveCollisions(particle);
            ComputeNextVelocity(particle);
        }
    }

    private void ResolveCollisions(Particle particle)
    {
        var pos = particle.Position;
        if (pos.x < 0) pos.x = 0;
        if (pos.x > width) pos.x = width;
        if (pos.y < 0) pos.y = 0;
        if (pos.y > height) pos.y = height;
        particle.Position = pos;
    }

    private void ComputeNextVelocity(Particle particle)
    {
        particle.Velocity = (particle.Position - particle.PreviousPosition) / Time.deltaTime;
    }

    private void DoubleDensityRelaxation(Particle particle)
    {
        var density = 0.0f;
        var nearDensity = 0.0f;
        // compute density and near-density
        var neighbors = hashTable.FindNeighbors(particle, h);
        foreach (var neighbor in neighbors)
        {
            var r = particle.Position - neighbor.Position;
            var q = r.magnitude / h;
            if (q < 1)
            {
                density += Mathf.Pow(1 - q, 2);
                nearDensity += Mathf.Pow(1 - q, 3);
            }
        }
        // compute pressure and near-pressure
        var p = stiffness * (density - restDensity);
        var pNear = nearStiffness * nearDensity;
        var dx = Vector2.zero;
        foreach (var neighbor in neighbors)
        {
            var r = particle.Position - neighbor.Position;
            var q = r.magnitude / h;
            if (q < 1)
            {
                // apply displacements
                var d = Time.deltaTime * Time.deltaTime * (p * (1 - q) + pNear * Mathf.Pow(1 - q, 2)) *
                        r.normalized;
                neighbor.Position += d / 2;
                dx -= d / 2;
            }
            
        }
        particle.Position += dx;
    }

    private void ApplyGravity(Particle particle)
    {
        particle.Velocity += new Vector2(0, gravity) * Time.deltaTime;
    }

    private void AdvanceToPredictedPosition(Particle particle)
    {
        particle.PreviousPosition = particle.Position;
        particle.Position += particle.Velocity * Time.deltaTime;
    }

    private void Update()
    {
        Simulate();
    }

    private void OnDrawGizmos()
    {
        foreach (var particle in Particles)
        {
            Gizmos.DrawSphere(particle.Position, 1.0f);
        }
    }
}
