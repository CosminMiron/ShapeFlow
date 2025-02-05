using System.Collections.Generic;
using CFD.GAS;
using UnityEngine;

public class VoxelizedData
{
    List<Vector3Int> _gridPoints = new List<Vector3Int>();
    private HashSet<Vector3Int> _hash = new HashSet<Vector3Int>();

    private float _halfSize;

    int xGridSize;
    int yGridSize;
    int zGridSize;

    public float HalfSize { get => _halfSize; }
    public List<Vector3Int> GridPoints { get => _gridPoints; set => _gridPoints = value; }
    public HashSet<Vector3Int> Hash { get => _hash; set => _hash = value; }

    public VoxelizedData(List<Vector3Int> gridPoints, HashSet<Vector3Int> hash, float halfSize, int x, int y, int z)
    {
        _gridPoints = new List<Vector3Int>(gridPoints);
        _hash = new HashSet<Vector3Int>(hash);
        _halfSize = halfSize;
        xGridSize = x;
        yGridSize = y;
        zGridSize = z;
    }

    public VoxelizedData GetVoxelizedMutation(List<VoxelAction> actions)
    {
        var data = new VoxelizedData(_gridPoints, _hash, _halfSize, xGridSize, yGridSize, zGridSize);
        data.ApplyActions(actions);
        return data;
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
                    if (!_hash.Contains(new Vector3Int(x, y, z))) continue;

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
        if (_hash.Contains(position + forward) || !_hash.Contains(position + forward * -1))
        {
            return 0f;
        }

        if (_hash.Contains(position + forward + new Vector3Int(0, 1, 0)) || _hash.Contains(position + forward + new Vector3Int(0, -1, 0)) || _hash.Contains(position + forward + new Vector3Int(1, 0, 0)) || _hash.Contains(position + forward + new Vector3Int(-1, 0, 0)))
        {
            return 0.05f;
        }

        return 0.2f;
    }

    public float CalculateDragCoefficient(Vector3Int forward, float speed = 10f, float airDensity = 1.2f)
    {
        var df = CalculateObjectDragForce(forward, speed, airDensity);
        var area = CalculateFrontalArea();

        return (2 * df) / (airDensity * (speed * speed) * area);
    }

    public void RemovePoint(Vector3Int point)
    {
        if (!_hash.Contains(point)) return;

        _gridPoints.Remove(point);
        _hash.Remove(point);
    }

    public void AddPoint(Vector3Int point)
    {
        if (!_hash.Contains(point)) return;

        _gridPoints.Add(point);
        _hash.Add(point);
    }

    private void ApplyActions(List<VoxelAction> actions)
    {
        foreach (var action in actions)
        {
            if (!_hash.Contains(action.position + action.direction))
            {
                AddPoint(action.position + action.direction);
                continue;
            }

            RemovePoint(action.position);
        }
    }
}
