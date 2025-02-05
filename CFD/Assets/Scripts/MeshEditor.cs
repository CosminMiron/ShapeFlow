using System.Collections.Generic;
using CFD.GAS;
using UnityEngine;

public class MeshEditor : MonoBehaviour
{
    [SerializeField] private VoxelizedMesh _voxelizedMesh;
    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private MeshRenderer _meshRenderer;

    [ContextMenu("Ceva")]
    private void Ceva()
    {
        var data = _voxelizedMesh.Voxelize();

        var halfSize = data.HalfSize * 2f;
        _meshFilter.sharedMesh.RecalculateBounds();

        var vertices = _meshFilter.mesh.vertices;
        var mbounds = _meshFilter.mesh.bounds;
        var unit = halfSize;
        var hunit = data.HalfSize;
        var voxelSize = Vector3.one * unit;

        var start = mbounds.min - new Vector3(hunit, hunit, hunit);

        var vertHash = new HashSet<Vector3>();

        foreach (var poz in data.GridPoints)
        {
            if (poz.y == 25 || poz.y == 24 || poz.y == 23 || poz.y == 22 || poz.y == 21 || poz.y == 20 || poz.y == 28 || poz.y == 27 || poz.y == 26)
            {
                var p = new Vector3(poz.x, poz.y, poz.z) * unit + start;
                var bounds = new Bounds(p, voxelSize);

                for (int i = 0, n = vertices.Length; i < n; i++)
                {
                    if (bounds.Contains(vertices[i]))
                    {
                        if (!vertHash.Contains(vertices[i]))
                        {
                            vertHash.Add(vertices[i]);
                        }
                    }
                }
            }
        }

        var offset = new Vector3(0, -unit * 2, 0);

        for (int i = 0, n = vertices.Length; i < n; i++)
        {
            if (vertHash.Contains(vertices[i]))
            {
                vertices[i] += offset;
            }
        }

        Mesh mesh = new Mesh();
        mesh.SetVertices(vertices);
        mesh.normals = _meshFilter.mesh.normals;
        mesh.tangents = _meshFilter.mesh.tangents;
        mesh.uv = _meshFilter.mesh.uv;
        mesh.uv2 = _meshFilter.mesh.uv2;
        var count = _meshFilter.mesh.subMeshCount;

        for (int i = 0; i < count; i++)
        {
            mesh.subMeshCount = count;
            var trg = _meshFilter.mesh.GetTriangles(i);
            mesh.SetTriangles(trg, i);
            var submesh = _meshFilter.mesh.GetSubMesh(i);
            mesh.SetSubMesh(i, submesh);
        }

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        GameObject go = new GameObject("Voxelized");
        go.transform.parent = transform;
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;
        go.transform.localRotation = Quaternion.identity;

        var filter = go.AddComponent<MeshFilter>();
        var renderer = go.AddComponent<MeshRenderer>();

        filter.mesh = mesh;
        renderer.materials = _meshRenderer.sharedMaterials;
    }

    public Vector3[] MoveVoxel(VoxelizedData data, Mesh mesh, Vector3[] vertices, List<VoxelAction> actions)
    {
        var halfSize = data.HalfSize * 2f;

        var mbounds = mesh.bounds;
        var unit = halfSize;
        var hunit = data.HalfSize;
        var voxelSize = Vector3.one * unit;

        var start = mbounds.min - new Vector3(hunit, hunit, hunit);


        foreach (var action in actions)
        {
            if (!data.Hash.Contains(action.position)) continue;

            var p = new Vector3(action.position.x, action.position.y, action.position.z) * unit + start;
            var bounds = new Bounds(p, voxelSize);

            for (int i = 0, n = vertices.Length; i < n; i++)
            {
                if (bounds.Contains(vertices[i]))
                {
                    var direction = new Vector3(action.direction.x, action.direction.y, action.direction.z);
                    var offset = direction * data.HalfSize * 4;

                    vertices[i] += offset;
                }
            }
        }

        return vertices;
    }

    public Mesh CreateMesh(Vector3[] vertices, Mesh startMesh)
    {
        Mesh mesh = new Mesh();
        mesh.SetVertices(vertices);
        mesh.normals = startMesh.normals;
        mesh.tangents = startMesh.tangents;
        mesh.uv = startMesh.uv;
        mesh.uv2 = startMesh.uv2;
        var count = startMesh.subMeshCount;

        for (int i = 0; i < count; i++)
        {
            mesh.subMeshCount = count;
            var trg = startMesh.GetTriangles(i);
            mesh.SetTriangles(trg, i);
            var submesh = startMesh.GetSubMesh(i);
            mesh.SetSubMesh(i, submesh);
        }

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        return mesh;
    }

    public void CreateNewMesh(Vector3[] vertices, Mesh startMesh)
    {
        Mesh mesh = new Mesh();
        mesh.SetVertices(vertices);
        mesh.normals = startMesh.normals;
        mesh.tangents = startMesh.tangents;
        mesh.uv = startMesh.uv;
        mesh.uv2 = startMesh.uv2;
        var count = startMesh.subMeshCount;

        for (int i = 0; i < count; i++)
        {
            mesh.subMeshCount = count;
            var trg = startMesh.GetTriangles(i);
            mesh.SetTriangles(trg, i);
            var submesh = startMesh.GetSubMesh(i);
            mesh.SetSubMesh(i, submesh);
        }

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        GameObject go = new GameObject("Voxelized");
        go.transform.localPosition = transform.position + new Vector3(4, 0, 0);
        go.transform.localScale = Vector3.one;
        go.transform.localRotation = Quaternion.identity;

        var filter = go.AddComponent<MeshFilter>();
        var renderer = go.AddComponent<MeshRenderer>();

        filter.mesh = mesh;
        renderer.materials = _meshRenderer.sharedMaterials;
    }
}
