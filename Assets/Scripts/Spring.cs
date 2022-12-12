public class Spring
{
    public Particle Particle1 { get; set; }
    public Particle Particle2 { get; set; }
    public float RestLength { get; set; }

    public Spring(Particle particle1, Particle particle2, float restLength)
    {
        Particle1 = particle1;
        Particle2 = particle2;
        RestLength = restLength;
    }
}