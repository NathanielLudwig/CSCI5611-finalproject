using System.Collections.Generic;
using UnityEngine;

public class SpatialHashTable
{
     private Cell[,] grid;
     public int Size;
     public int CellSize;

    public SpatialHashTable(int size, int cellSize)
    {
        grid = new Cell[size, size];
        Size = size;
        CellSize = cellSize;
    }
    
    private Vector2Int CalculateIndex(Vector2 position)
    {
        // Divide the position by the size of the cell and round down to the nearest integer
        int x = Mathf.FloorToInt(position.x / CellSize);
        int y = Mathf.FloorToInt(position.y / CellSize);

        // Return the indices as a Vector2Int
        return new Vector2Int(x, y);
    }

    public List<Particle> FindNeighbors(Particle particle, float h)
    {
        // Create a list to store the neighbors
        List<Particle> neighbors = new List<Particle>();

        // Calculate the 3D index of the particle
        Vector2Int index = CalculateIndex(particle.Position);

        // Iterate over all the neighboring cells
        for (int x = index.x - 1; x <= index.x + 1; x++)
        {
            for (int y = index.y - 1; y <= index.y + 1; y++)
            {
                if (x < 0 || x >= Size || y < 0 || y >= Size) continue;
                // Check if the neighboring cell exists in the hash table
                if (this[x, y] == null) continue;
                // If the cell exists, iterate over all the particles in the cube
                foreach (Particle neighbor in this[x, y].Particles)
                {
                    // Check if the particle is within the interaction radius of the given particle
                    if (Vector2.Distance(particle.Position, neighbor.Position) <= h)
                    {
                        // If the particle is within the interaction radius, add it to the list of neighbors
                        neighbors.Add(neighbor);
                    }
                }
            }
        }
        // Return the list of neighbors
        return neighbors;
    }

    public void Add(Particle particle)
    {
        Vector2Int index = CalculateIndex(particle.Position);

        if (this[index.x, index.y] == null)
        {
            Cell cell = new Cell();
            AddCell(index.x, index.y, cell);
        }

        this[index.x, index.y].AddParticle(particle);
    }

    public void Clear()
    {
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (this[x, y] == null) continue;
                RemoveCell(x, y);
            }
        }
    }
    
    private void AddCell(int x, int y, Cell cell)
    {
        grid[x, y] = cell;
    }

    public void RemoveCell(int x, int y)
    {
        grid[x, y] = null;
    }

    public Cell this[int x, int y]
    {
        get { return grid[x, y]; }
    }
}