using System;
using System.Collections.Generic;
using UnityEngine;

public class Fluid : MonoBehaviour
{
    public List<Particle> Particles = new();
    public List<GameObject> ParticleObjects = new();

    public GameObject CirclePrefab;
    public GameObject ObstaclePrefab;
    public int maxParticles = 1000;
    public float gravity = -1.0f;
    public float stiffness = 0.008f;
    public float h = 3.0f;
    public int gridSize = 30;
    public int cellSize = 3;
    private SpatialHashTable hashTable;
    public float nearStiffness = 0.01f;
    public float restDensity = 10.0f;
    public Vector2 spherePos = new(30, 10);
    public float sphereRadius = 2f;
    public float width = 10;
    public float height = 10;
    public Camera camera;
    
    // Start is called before the first frame update
    private void Start()
    {
        camera = Camera.main;
        hashTable = new SpatialHashTable(gridSize, cellSize);
        var obstacle = Instantiate(ObstaclePrefab, new Vector3(spherePos.x, spherePos.y, 0), Quaternion.identity);
        obstacle.transform.localScale = new Vector3(2 * sphereRadius, 2 * sphereRadius, 0);
        for (int x = 1; x < 30; x++)
        {
            for (int y = 1; y < 20; y++)
            {
                Particles.Add(new Particle(new Vector2(x, y), Vector2.zero));
                ParticleObjects.Add(Instantiate(CirclePrefab, new Vector3(x, y, 0), Quaternion.identity));
            }
        }
    }

    private void Simulate(float dt)
    {
        hashTable.Clear();
        // Pass 1
        foreach (var particle in Particles)
        {
            ApplyGravity(particle, dt);
            AdvanceToPredictedPosition(particle, dt);
            ResolveCollisions(particle);
            hashTable.Add(particle);
        }

        // Pass 2
        foreach (var particle in Particles)
        {
           DoubleDensityRelaxation(particle, dt); 
        }

        // Pass 3
        foreach (var particle in Particles)
        {
            ResolveCollisions(particle);
            ComputeNextVelocity(particle, dt);
        }

        for (int i = 0; i < Particles.Count; i++)
        {
            ParticleObjects[i].transform.position = new Vector3(Particles[i].Position.x, Particles[i].Position.y, 0);
        }
    }

    private void ResolveCollisions(Particle particle)
    {
        var pos = particle.Position;
        if (pos.x < 0) pos.x = 0;
        if (pos.x > width) pos.x = width;
        if (pos.y < 0) pos.y = 0;
        if (pos.y > height) pos.y = height;
        if (Vector2.Distance(pos, spherePos) < sphereRadius+1.1f) {
            Vector2 normal = (pos - spherePos).normalized;
            pos = (spherePos + normal * (sphereRadius+1.1f)) * 1.01f;
        }
        particle.Position = pos;
    }

    private void ComputeNextVelocity(Particle particle, float dt)
    {
        particle.Velocity = (particle.Position - particle.PreviousPosition) / dt;
    }

    private void DoubleDensityRelaxation(Particle particle, float dt)
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
            var r = neighbor.Position - particle.Position;
            var q = r.magnitude / h;
            if (q < 1)
            {
                // apply displacements
                var d = dt * dt * (p * (1 - q) + pNear * Mathf.Pow(1 - q, 2)) *
                        r.normalized;
                neighbor.Position += d / 2;
                dx -= d / 2;
            }
            
        }
        particle.Position += dx;
    }

    private void ApplyGravity(Particle particle, float dt)
    {
        Vector3 mousePos = Input.mousePosition;
        var worldMousePos = camera.ScreenToWorldPoint(mousePos);
        particle.Velocity += new Vector2(0, gravity) * dt;
        var fromMouse =  particle.Position - new Vector2(worldMousePos.x, worldMousePos.y);
        particle.Velocity += fromMouse.normalized * dt;
    }

    private void AdvanceToPredictedPosition(Particle particle, float dt)
    {
        particle.PreviousPosition = particle.Position;
        particle.Position += particle.Velocity * dt;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            var worldMousePos = camera.ScreenToWorldPoint(mousePos);
            Particles.Add(new Particle(new Vector2(worldMousePos.x, worldMousePos.y), Vector2.zero));
            ParticleObjects.Add(Instantiate(CirclePrefab, new Vector3(worldMousePos.x, worldMousePos.y, 0), Quaternion.identity));
        }
    }

    void FixedUpdate()
    {
        Simulate(Time.fixedDeltaTime);
    }
}
