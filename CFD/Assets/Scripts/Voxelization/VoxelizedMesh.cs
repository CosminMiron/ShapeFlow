using System.Collections.Generic;
using UnityEngine;

public class VoxelizedMesh : MonoBehaviour
{
    public List<Vector3Int> GridPoints = new List<Vector3Int>();
    public float HalfSize = 0.1f;
    public Vector3 LocalOrigin;
    public MeshFilter MeshFilter;

    private HashSet<Vector3Int> hash = new HashSet<Vector3Int>();

    public Vector3 PointToPosition(Vector3 point)
    {
        float size = HalfSize * 2f;
        Vector3 pos = new Vector3(HalfSize + point.x * size, HalfSize + point.y * size, HalfSize + point.z * size);
        return LocalOrigin + transform.TransformPoint(pos);
    }

    public void OnDrawGizmosSelected()
    {
        float size = HalfSize * 2f;
        foreach (Vector3Int gridPoint in GridPoints)
        {
            Gizmos.color = Color.green;

            if (!hash.Contains(gridPoint + new Vector3Int(0, 0, 1)) && hash.Contains(gridPoint + new Vector3Int(0, 0, -1)))
            {
                Gizmos.color = Color.red;
            }

            Vector3 worldPos = PointToPosition(gridPoint);
            Gizmos.DrawWireCube(worldPos, new Vector3(size, size, size));
            Gizmos.color = Color.red;
            Gizmos.DrawLine(worldPos, worldPos + new Vector3(0, 0, HalfSize));
        }

        Gizmos.color = Color.white;

        if (TryGetComponent(out MeshCollider meshCollider))
        {
            Bounds bounds = meshCollider.bounds;
            Gizmos.DrawWireCube(bounds.center, bounds.extents * 2);
        }
    }

    [ContextMenu("Voxelize")]
    public void Voxelize()
    {
        VoxelizeMesh(MeshFilter);
    }

    public void VoxelizeMesh(MeshFilter meshFilter)
    {
        if (!meshFilter.TryGetComponent(out MeshCollider meshCollider))
        {
            meshCollider = meshFilter.gameObject.AddComponent<MeshCollider>();
        }

        Bounds bounds = meshCollider.bounds;
        Vector3 minExtents = bounds.center - bounds.extents;
        Vector3 count = bounds.extents / HalfSize;

        int xGridSize = Mathf.CeilToInt(count.x);
        int yGridSize = Mathf.CeilToInt(count.y);
        int zGridSize = Mathf.CeilToInt(count.z);

        GridPoints.Clear();
        hash.Clear();
        LocalOrigin = transform.InverseTransformPoint(minExtents);

        for (int x = 0; x < xGridSize; ++x)
        {
            for (int z = 0; z < zGridSize; ++z)
            {
                for (int y = 0; y < yGridSize; ++y)
                {
                    Vector3 pos = PointToPosition(new Vector3Int(x, y, z));
                    if (Physics.CheckBox(pos, new Vector3(HalfSize, HalfSize, HalfSize)))
                    {
                        GridPoints.Add(new Vector3Int(x, y, z));
                        hash.Add(new Vector3Int(x, y, z));
                    }
                }
            }
        }

        Vector3Int[] faceOffsets = new Vector3Int[]
{
            new Vector3Int(1, 0, 0),  // Right
            new Vector3Int(-1, 0, 0), // Left
            new Vector3Int(0, 1, 0),  // Up
            new Vector3Int(0, -1, 0), // Down
            new Vector3Int(0, 0, 1),  // Forward
            new Vector3Int(0, 0, -1)  // Backward
};

        var gridWithLessNegh = new List<Vector3Int>();

        foreach (var item in GridPoints)
        {
            int neighborCount = 0;
            foreach (Vector3Int offset in faceOffsets)
            {
                Vector3Int neighborPosition = item + offset;
                if (hash.Contains(neighborPosition))
                {
                    neighborCount++;
                }
            }

            if (neighborCount == 6)
            {
                continue;
            }

            gridWithLessNegh.Add(item);
        }

        GridPoints = gridWithLessNegh;

        var ceva = new VoxelizedData(GridPoints, hash, HalfSize, xGridSize, yGridSize, zGridSize);

        Debug.LogError(ceva.CalculateObjectDragForce());
        var result = ceva.CalculateFrontalArea();
        Debug.LogError(result);

        Debug.LogError(ceva.CalculateDragCoefficient());
    }
}