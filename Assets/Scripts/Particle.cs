using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle
{
    // Start is called before the first frame update
    public Vector2 Position { get; set; }
    public Vector2 PreviousPosition { get; set; }
    public Vector2 Velocity { get; set; }

    public Particle(Vector2 position, Vector2 velocity)
    {
        Position = position;
        Velocity = velocity;
    }
}
