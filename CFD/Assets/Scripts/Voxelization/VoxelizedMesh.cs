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
    public Vector3Int forward = new Vector3Int(0, 0, 1);
    public float Speed = 30;
    public float AirDensity = 1.2f;
    public VoxelizedData VoxelizedData;
    public int resolution = 100;

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
    public VoxelizedData Voxelize()
    {
        GridPoints.Clear();
        CPUVoxelizer.Voxelize(MeshFilter.sharedMesh, resolution, out var ceva, out var unit, out var x, out var y, out var z);

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
        VoxelizedData = new VoxelizedData(GridPoints, hash, HalfSize, x, y, z);

        Debug.LogError("DF  " + VoxelizedData.CalculateObjectDragForce(forward, Speed, AirDensity));
        var result = VoxelizedData.CalculateFrontalArea();
        Debug.LogError("FA  " + result);

        Debug.LogError("DC  " + VoxelizedData.CalculateDragCoefficient(forward, Speed, AirDensity));

        return VoxelizedData;

    }

    public VoxelizedData Voxelize(int resolution)
    {
        GridPoints.Clear();
        CPUVoxelizer.Voxelize(MeshFilter.sharedMesh, resolution, out var ceva, out var unit, out var x, out var y, out var z);

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
        VoxelizedData = new VoxelizedData(GridPoints, hash, HalfSize, x, y, z);

        return VoxelizedData;
    }

    private void ShowDiff(List<Vector3Int> newList)
    {
        var points = new List<Vector3Int>();
        foreach (var element in newList)
        {
            if (GridPoints.Contains(element)) continue;

            points.Add(element);
        }

        foreach (var element in GridPoints)
        {
            if (!newList.Contains(element))
            {
                points.Add(element);
            }
        }

        GridPoints = points;
    }
}