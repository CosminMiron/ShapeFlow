using System.Collections.Generic;
using UnityEngine;
using VoxelSystem;

public class VoxelizedMesh : MonoBehaviour
{
    public List<Vector3Int> GridPoints = new List<Vector3Int>();
    public float HalfSize = 0.1f;
    public Vector3 LocalOrigin;
    public MeshFilter MeshFilter;
    public bool _showGrid;
    public Vector3Int forward = new Vector3Int(0,0,1);

    private HashSet<Vector3Int> hash = new HashSet<Vector3Int>();

    public Vector3 PointToPosition(Vector3 point)
    {
        float size = HalfSize * 2f;
        Vector3 pos = new Vector3(HalfSize + point.x * size, HalfSize + point.y * size, HalfSize + point.z * size);
        return LocalOrigin + transform.TransformPoint(pos);
    }

    public void OnDrawGizmosSelected()
    {
        if (!_showGrid) return;

        float size = HalfSize * 2f;
        foreach (Vector3Int gridPoint in GridPoints)
        {
            Gizmos.color = Color.green;

            if (!hash.Contains(gridPoint + forward) && hash.Contains(gridPoint + forward * -1))
            {
                Gizmos.color = Color.red;
            }

            //  if (gridPoint.y == 25 ||gridPoint.y == 24 ||gridPoint.y == 23 || gridPoint.y == 22 || gridPoint.y == 21|| gridPoint.y == 28 || gridPoint.y == 29|| gridPoint.y == 26|| gridPoint.y == 27)
            // {
            //     Gizmos.color = Color.blue;
            // }

            Vector3 worldPos = PointToPosition(gridPoint);
            Gizmos.DrawWireCube(worldPos, new Vector3(size, size, size));
        }

        Gizmos.color = Color.white;

        if (TryGetComponent(out MeshCollider meshCollider))
        {
            Bounds bounds = meshCollider.bounds;
            Gizmos.DrawWireCube(bounds.center, bounds.extents * 2);
        }
    }

    [ContextMenu("Voxelize")]
    public VoxelizedData Voxelize2()
    {
        //VoxelizeMesh(MeshFilter);
        GridPoints.Clear();
        CPUVoxelizer.Voxelize(MeshFilter.sharedMesh, 100, out var ceva, out var unit, out var x, out var y, out var z);

        GridPoints = ceva;
        foreach (var p in ceva)
        {
            hash.Add(p);
        }
        HalfSize = unit / 2f;
        if (!MeshFilter.TryGetComponent(out MeshCollider meshCollider))
        {
            meshCollider = MeshFilter.gameObject.AddComponent<MeshCollider>();
        }
        Bounds bounds = meshCollider.bounds;
        Vector3 minExtents = bounds.center - bounds.extents;
        LocalOrigin = transform.InverseTransformPoint(minExtents);
        var ceva2 = new VoxelizedData(GridPoints, hash, HalfSize, x, y, z);

        Debug.LogError("DF  " +ceva2.CalculateObjectDragForce(forward));
        var result = ceva2.CalculateFrontalArea();
        Debug.LogError("FA  " +result);

        Debug.LogError("DC  " + ceva2.CalculateDragCoefficient(forward));

        return ceva2;

    }

    public VoxelizedData VoxelizeMesh(MeshFilter meshFilter)
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

        Debug.LogError(ceva.CalculateObjectDragForce(forward));
        var result = ceva.CalculateFrontalArea();
        Debug.LogError(result);

        Debug.LogError(ceva.CalculateDragCoefficient(forward));

        return ceva;
    }
}