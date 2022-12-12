using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle
{
    // Start is called before the first frame update
    public Vector3 Position { get; set; }
    public Vector3 PreviousPosition { get; set; }
    public Vector3 Velocity { get; set; }

    public Particle(Vector3 position, Vector3 velocity)
    {
        Position = position;
        Velocity = velocity;
    }
}
