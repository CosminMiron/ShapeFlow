using System.Collections.Generic;
using UnityEngine;

public class VoxelizedData
{
    List<Vector3Int> GridPoints = new List<Vector3Int>();
    private HashSet<Vector3Int> hash = new HashSet<Vector3Int>();

    private float HalfSize;
    private Vector3 LocalOrigin;
    private float density = 1.2f;
    private float speed = 10;

    int xGridSize;
    int yGridSize;
    int zGridSize;

    Vector3Int[] faceOffsets = new Vector3Int[]
{
            new Vector3Int(1, 0, 0),  // Right
            new Vector3Int(-1, 0, 0), // Left
            new Vector3Int(0, 1, 0),  // Up
            new Vector3Int(0, -1, 0), // Down
            new Vector3Int(0, 0, 1),  // Forward
            new Vector3Int(0, 0, -1)  // Backward
};

    public VoxelizedData(List<Vector3Int> gridPoints, HashSet<Vector3Int> hash, float halfSize, int x, int y, int z)
    {
        GridPoints = gridPoints;
        this.hash = hash;
        HalfSize = halfSize;
        xGridSize = x;
        yGridSize = y;
        zGridSize = z;
    }

    public float  CalculateFrontalArea()
    {
        var ceva = new bool[xGridSize, yGridSize, zGridSize];

        for (int x = 0; x < xGridSize; x++)
        {
            for (int y = 0; y < yGridSize; y++)
            {
                for (int z = 0; z < zGridSize; z++)
                {
                    ceva[x, y, z] = false;
                }
            }
        }
        for (int x = 0; x < xGridSize; x++)
        {
            for (int y = 0; y < yGridSize; y++)
            {
                for (int z = 0; z < zGridSize; z++)
                {
                    if (!hash.Contains(new Vector3Int(x, y, z))) continue;

                    ceva[x, y, z] = true;
                }
            }
        }

        var area = 0f;

        for (int x = 0; x < xGridSize; x++)
        {
            for (int y = 0; y < yGridSize; y++)
            {
                for (int z = 0; z < zGridSize; z++)
                {
                    if (ceva[x, y, z])
                    {
                        area += (HalfSize * 2) * (HalfSize * 2);
                        break;
                    }
                }
            }
        }

        return  area;
    }

    public float CalculateObjectDragForce()
    {
        var force = 0f;
        foreach (var point in GridPoints)
        {
            force += 0.5f * density * (speed * speed) * CalculatePointDragForce(point) * (HalfSize * 2) * (HalfSize * 2);
        }

        return force;
    }

    public float CalculatePointDragForce(Vector3Int position)
    {
        if (!hash.Contains(position + new Vector3Int(0, 0, 1)) && hash.Contains(position + new Vector3Int(0, 0, -1)))
        {
            return 0.2f;
        }

        return 0f;
    }

    public float CalculateDragCoefficient()
    {
        var df = CalculateObjectDragForce();
        var area = CalculateFrontalArea();

        return (2 * df)/ (density * (speed * speed) * area);
    }

}
