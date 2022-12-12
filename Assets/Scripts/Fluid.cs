using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Fluid : MonoBehaviour
{
    public List<Particle> Particles { get; set; }
    public List<Spring> Springs { get; set; }

    public float gravity = -9.8f;
    public float gamma = 1.0f;
    public float alpha = 0.3f;
    public float k = 0.004f;
    public float h = 1.0f;
    // Start is called before the first frame update
    private void Start()
    {
        
    }

    private void Simulate()
    {
        ApplyGravity();
        AdvanceToPredictedPosition();
        AdjustSprings();
        ApplySpringDisplacements();
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
        
    }
}
