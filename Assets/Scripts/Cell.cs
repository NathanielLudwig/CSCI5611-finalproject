using System.Collections.Generic;

public class Cell
{
    // A list to store the particles in this cube
    public List<Particle> Particles { get; }

    // Constructor to initialize the list of particles
    public Cell()
    {
        Particles = new List<Particle>();
    }

    // Method to add a particle to the cube
    public void AddParticle(Particle particle)
    {
        Particles.Add(particle);
    }

    // Method to remove a particle from the cube
    public void RemoveParticle(Particle particle)
    {
        Particles.Remove(particle);
    }
}