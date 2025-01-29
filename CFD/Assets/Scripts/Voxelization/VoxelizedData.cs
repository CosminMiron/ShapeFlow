using System.Collections.Generic;
using UnityEngine;

public class VoxelizedData
{
    List<Vector3Int> _gridPoints = new List<Vector3Int>();
    private HashSet<Vector3Int> hash = new HashSet<Vector3Int>();

    private float _halfSize;

    int xGridSize;
    int yGridSize;
    int zGridSize;

    public float HalfSize { get => _halfSize; }
    public List<Vector3Int> GridPoints { get => _gridPoints; set => _gridPoints = value; }

    public VoxelizedData(List<Vector3Int> gridPoints, HashSet<Vector3Int> hash, float halfSize, int x, int y, int z)
    {
        _gridPoints = gridPoints;
        this.hash = hash;
        _halfSize = halfSize;
        xGridSize = x;
        yGridSize = y;
        zGridSize = z;
    }

    public float CalculateFrontalArea()
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
                        area += (_halfSize * 2) * (_halfSize * 2);
                        break;
                    }
                }
            }
        }

        return area;
    }

    public float CalculateObjectDragForce(Vector3Int forward, float speed, float airDensity)
    {
        var force = 0f;
        foreach (var point in _gridPoints)
        {
            force += 0.5f * airDensity * (speed * speed) * CalculatePointDragForce(point, forward) * (_halfSize * 2) * (_halfSize * 2);
        }

        return force;
    }

    public float CalculatePointDragForce(Vector3Int position, Vector3Int forward)
    {
        if (hash.Contains(position + forward) || !hash.Contains(position + forward * -1))
        {
            return 0f;
        }

        if (hash.Contains(position + forward + new Vector3Int(0, 1, 0)) || hash.Contains(position + forward + new Vector3Int(0, -1, 0)) || hash.Contains(position + forward + new Vector3Int(1, 0, 0)) || hash.Contains(position + forward + new Vector3Int(-1, 0, 0)))
        {
            return 0.05f;
        }

        return 0.2f;
    }

    public float CalculateDragCoefficient(Vector3Int forward, float speed, float airDensity)
    {
        var df = CalculateObjectDragForce(forward, speed, airDensity);
        var area = CalculateFrontalArea();

        return (2 * df) / (airDensity * (speed * speed) * area);
    }

}
