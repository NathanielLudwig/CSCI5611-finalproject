public class HashTable
{
    // An array to store the cubes in the hash table
     private Cube[,,] cubes;
     public int Size;

    // Constructor to initialize the array of cubes
    public HashTable(int size)
    {
        cubes = new Cube[size, size, size];
        Size = size;
    }

    // Method to add a cube to the hash table
    public void AddCube(int x, int y, int z, Cube cube)
    {
        cubes[x, y, z] = cube;
    }

    // Method to remove a cube from the hash table
    public void RemoveCube(int x, int y, int z)
    {
        cubes[x, y, z] = null;
    }

    // Property to get a cube from the hash table
    public Cube this[int x, int y, int z]
    {
        get { return cubes[x, y, z]; }
    }
}