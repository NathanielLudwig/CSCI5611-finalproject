using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Fluid : MonoBehaviour
{
    public List<Particle> Particles = new();
    public List<Spring> Springs = new();
    private HashTable hashTable = new HashTable(30);

    public int maxParticles = 1000;
    public float gravity = -1.0f;
    public float gamma = 1.0f;
    public float alpha = 0.3f;
    public float k = 0.008f;
    public float h = 3.0f;
    public float cubeSize = 1.0f;
    public float kNear = 0.01f;
    public float restRho = 10.0f;

    public float width = 10;
    public float height = 10;
    public float depth = 10;
    // Start is called before the first frame update
    private void Start()
    {
        // var x = 0.0f;
        // for (int i = 0; i < maxParticles; i++)
        // {
        //     Particles.Add(new Particle(
        //         new Vector3(Random.Range(0, width), Random.Range(0, height), Random.Range(0, depth)), Vector3.zero));
        //     x += 0.1f;
        // }
        for (int x = 0; x < 10; x++)
        {
            for (int y = 1; y < 10; y++)
            {
                for (int z = 0; z < 10; z++)
                {
                    Particles.Add(new Particle(new Vector3(x, y, z), Vector3.zero));
                }
            }
        }
    //     var y = 5.0f;
    //     for (int i = 0; i < maxParticles; i++)
    //     {
    //         Particles.Add(new Particle(
    //             new Vector3(10, y, 10), Vector3.zero));
    //         y += 0.1f;
    //     }
    }

    private void Simulate()
    {
        ApplyGravity();
        AdvanceToPredictedPosition();
        // AdjustSprings();
        // ApplySpringDisplacements();
        DoubleDensityRelaxation();
        ResolveCollisions();
        ComputeNextVelocity();
        UpdateParticleInHashTable();
    }

    private void UpdateParticleInHashTable()
    {
        // Iterate over all the particles
        foreach (Particle particle in Particles)
        {
            // Calculate the 3D index of the particle
            Vector3Int index = CalculateIndex(particle.Position, cubeSize);

            // Check if the corresponding cube exists in the hash table
            if (hashTable[index.x, index.y, index.z] == null)
            {
                // If the cube does not exist, create a new one
                Cube cube = new Cube();
                hashTable.AddCube(index.x, index.y, index.z, cube);
            }
            else
            {
                // If the particle was previously in another cube, remove it from that cube
                Vector3Int oldIndex = CalculateIndex(particle.PreviousPosition, cubeSize);
                if (oldIndex != index)
                {
                    if (hashTable[oldIndex.x, oldIndex.y, oldIndex.z] != null)
                    {
                        hashTable[oldIndex.x, oldIndex.y, oldIndex.z].RemoveParticle(particle);
                    }
                }
            }

            // Add the particle to the corresponding cube
            hashTable[index.x, index.y, index.z].AddParticle(particle);
        }
    }

    private void ResolveCollisions()
    {
        foreach (var particle in Particles)
        {
            var pos = particle.Position;
            if (pos.x < 0) pos.x = 0;
            if (pos.x > width) pos.x = width;
            if (pos.y < 0) pos.y = 0;
            if (pos.y > height) pos.y = height;
            if (pos.z < 0) pos.z = 0;
            if (pos.z > depth) pos.z = depth;
            particle.Position = pos;
        }
    }

    private void ComputeNextVelocity()
    {
        foreach (var particle in Particles)
        {
            particle.Velocity = (particle.Position - particle.PreviousPosition) / Time.deltaTime;
        }
    }

    private void DoubleDensityRelaxation()
    {
        foreach (var particle in Particles)
        {
            var rho = 0.0f;
            var rhoNear = 0.0f;
            // compute density and near-density
            var neighbors = FindNeighbors(particle, cubeSize, h);
            foreach (var neighbor in neighbors)
            {
                var r = particle.Position - neighbor.Position;
                var q = r.magnitude / h;
                if (q < 1)
                {
                    rho += Mathf.Pow(1 - q, 2);
                    rhoNear += Mathf.Pow(1 - q, 3);
                }
            }
            // compute pressure and near-pressure
            var p = k * (rho - restRho);
            var pNear = kNear * rhoNear;
            var dx = Vector3.zero;
            foreach (var neighbor in Particles.Where(neighbor => particle != neighbor))
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
    }
    
    public Vector3Int CalculateIndex(Vector3 position, float cubeSize)
    {
        // Divide the position by the size of the cube and round down to the nearest integer
        int x = Mathf.FloorToInt(position.x / cubeSize);
        int y = Mathf.FloorToInt(position.y / cubeSize);
        int z = Mathf.FloorToInt(position.z / cubeSize);

        // Return the indices as a Vector3Int
        return new Vector3Int(x, y, z);
    }
    
    public List<Particle> FindNeighbors(Particle particle, float cubeSize, float h)
    {
        // Create a list to store the neighbors
        List<Particle> neighbors = new List<Particle>();

        // Calculate the 3D index of the particle
        Vector3Int index = CalculateIndex(particle.Position, cubeSize);

        // Iterate over all the neighboring cubes
        for (int x = index.x - 1; x <= index.x + 1; x++)
        {
            for (int y = index.y - 1; y <= index.y + 1; y++)
            {
                for (int z = index.z - 1; z <= index.z + 1; z++)
                {
                    if (x < 0 || x >= hashTable.Size || y < 0 || y >= hashTable.Size || z < 0 ||
                        z >= hashTable.Size) continue;
                    // Check if the neighboring cube exists in the hash table
                    if (hashTable[x, y, z] == null) continue;
                    // If the cube exists, iterate over all the particles in the cube
                    foreach (Particle neighbor in hashTable[x, y, z].Particles)
                    {
                        // Check if the particle is within the interaction radius of the given particle
                        if (Vector3.Distance(particle.Position, neighbor.Position) <= h)
                        {
                            // If the particle is within the interaction radius, add it to the list of neighbors
                            neighbors.Add(neighbor);
                        }
                    }
                }
            }
        }

        // Return the list of neighbors
        return neighbors;
    }

    private void ApplyGravity()
    {
        foreach (var particle in Particles)
        {
            particle.Velocity += new Vector3(0, gravity, 0) * Time.deltaTime;
        }
    }

    private void AdvanceToPredictedPosition()
    {
        foreach (var particle in Particles)
        {
            particle.PreviousPosition = particle.Position;
            particle.Position += particle.Velocity * Time.deltaTime;
        }
    }

    private void ApplySpringDisplacements()
    {
        foreach (var spring in Springs)
        {
            var r = spring.Particle2.Position - spring.Particle1.Position;
            var d = Time.deltaTime * Time.deltaTime * k * (1 - spring.RestLength / h) *
                    (spring.RestLength - r.magnitude) * r.normalized;
            spring.Particle1.Position -= d / 2;
            spring.Particle1.Position += d / 2;
        }
    }

    private void AdjustSprings()
    {
        foreach (var i in Particles)
        {
            foreach (var j in Particles)
            {
                if (i == j) break;
                var r = Vector3.Magnitude(j.Position - i.Position);
                if (r < h)
                {
                    var springOrNull = GetSpringBetweenParticles(i, j);
                    var spring = springOrNull ?? new Spring(i, j, h);
                    if (springOrNull == null)
                    {
                        Springs.Add(spring);
                    }
                    var d = gamma * spring.RestLength;
                    if (r > spring.RestLength + d)
                    {
                        spring.RestLength += Time.deltaTime * alpha * (r - spring.RestLength - d);
                    }
                    else if (r < spring.RestLength - d)
                    {
                        spring.RestLength -= Time.deltaTime * alpha * (spring.RestLength - d - r);
                    }
                }
            }
        }
        Springs.RemoveAll(spring => spring.RestLength < h);
    }

    private Spring GetSpringBetweenParticles(Particle particle1, Particle particle2)
    {
        // Find the spring connecting the two particles, if it exists
        return Springs.FirstOrDefault(spring =>
            (spring.Particle1 == particle1 && spring.Particle2 == particle2) ||
            (spring.Particle1 == particle2 && spring.Particle2 == particle1));
    }

    // void ApplyViscosity()
    // {
    //     
    // }

    // Update is called once per frame
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
