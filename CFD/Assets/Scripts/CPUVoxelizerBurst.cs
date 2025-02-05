// using Unity.Burst;
// using Unity.Collections;
// using Unity.Jobs;
// using UnityEngine;
// using VoxelSystem;
// using static VoxelSystem.CPUVoxelizer;

// [BurstCompile]
// public struct CPUVoxelizerBurst : IJobParallelFor
// {
//     public Bounds bounds;
//     [NativeDisableParallelForRestriction] public NativeArray<int> voxels;
//     public int resolution;
    
//     [NativeDisableParallelForRestriction] public NativeArray<Vector3> vertices;
//     [NativeDisableParallelForRestriction] public NativeArray<Vector2> uvs;
//     [NativeDisableParallelForRestriction] public NativeArray<int> indices;

//     public void Execute(int index)
//     {
//         float maxLength = Mathf.Max(bounds.size.x, Mathf.Max(bounds.size.y, bounds.size.z));
//         var unit = maxLength / resolution;
//         var hunit = unit * 0.5f;
//         var start = bounds.min - new Vector3(hunit, hunit, hunit);
//         var end = bounds.max + new Vector3(hunit, hunit, hunit);
//         var size = end - start;

//         var width = Mathf.CeilToInt(size.x / unit);
//         var height = Mathf.CeilToInt(size.y / unit);
//         var depth = Mathf.CeilToInt(size.z / unit);

//         var boxes = new Bounds[width, height, depth];
//         var voxelSize = Vector3.one * unit;
//         for (int x = 0; x < width; x++)
//         {
//             for (int y = 0; y < height; y++)
//             {
//                 for (int z = 0; z < depth; z++)
//                 {
//                     var p = new Vector3(x, y, z) * unit + start;
//                     var aabb = new Bounds(p, voxelSize);
//                     boxes[x, y, z] = aabb;
//                 }
//             }
//         }

//         // build triangles
//         var uv00 = Vector2.zero;
//         var direction = Vector3.forward;

//         if (index % 3 != 0) return;

//         var tri = new Triangle(
//             vertices[indices[index]],
//             vertices[indices[index + 1]],
//             vertices[indices[index + 2]],
//             direction
//         );

//         Vector2 uva, uvb, uvc;
//         if (uvs.Length > 0)
//         {
//             uva = uvs[indices[index]];
//             uvb = uvs[indices[index + 1]];
//             uvc = uvs[indices[index + 2]];
//         }
//         else
//         {
//             uva = uvb = uvc = uv00;
//         }

//         var min = tri.bounds.min - start;
//         var max = tri.bounds.max - start;
//         int iminX = Mathf.RoundToInt(min.x / unit), iminY = Mathf.RoundToInt(min.y / unit), iminZ = Mathf.RoundToInt(min.z / unit);
//         int imaxX = Mathf.RoundToInt(max.x / unit), imaxY = Mathf.RoundToInt(max.y / unit), imaxZ = Mathf.RoundToInt(max.z / unit);
//         // int iminX = Mathf.FloorToInt(min.x / unit), iminY = Mathf.FloorToInt(min.y / unit), iminZ = Mathf.FloorToInt(min.z / unit);
//         // int imaxX = Mathf.CeilToInt(max.x / unit), imaxY = Mathf.CeilToInt(max.y / unit), imaxZ = Mathf.CeilToInt(max.z / unit);

//         iminX = Mathf.Clamp(iminX, 0, width - 1);
//         iminY = Mathf.Clamp(iminY, 0, height - 1);
//         iminZ = Mathf.Clamp(iminZ, 0, depth - 1);
//         imaxX = Mathf.Clamp(imaxX, 0, width - 1);
//         imaxY = Mathf.Clamp(imaxY, 0, height - 1);
//         imaxZ = Mathf.Clamp(imaxZ, 0, depth - 1);
//         // Debug.Log((iminX + "," + iminY + "," + iminZ) + " ~ " + (imaxX + "," + imaxY + "," + imaxZ));

//         uint front = (uint)(tri.frontFacing ? 1 : 0);

//         for (int x = iminX; x <= imaxX; x++)
//         {
//             for (int y = iminY; y <= imaxY; y++)
//             {
//                 for (int z = iminZ; z <= imaxZ; z++)
//                 {
//                     if (Intersects(tri, boxes[x, y, z]))
//                     {
//                         voxels[x + y + z] = 1;
//                     }
//                 }
//             }
//         }
//     }

//     public static bool Intersects(Triangle tri, Bounds aabb)
//     {
//         float p0, p1, p2, r;

//         Vector3 center = aabb.center, extents = aabb.max - center;

//         Vector3 v0 = tri.a - center,
//             v1 = tri.b - center,
//             v2 = tri.c - center;

//         Vector3 f0 = v1 - v0,
//             f1 = v2 - v1,
//             f2 = v0 - v2;

//         Vector3 a00 = new Vector3(0, -f0.z, f0.y),
//             a01 = new Vector3(0, -f1.z, f1.y),
//             a02 = new Vector3(0, -f2.z, f2.y),
//             a10 = new Vector3(f0.z, 0, -f0.x),
//             a11 = new Vector3(f1.z, 0, -f1.x),
//             a12 = new Vector3(f2.z, 0, -f2.x),
//             a20 = new Vector3(-f0.y, f0.x, 0),
//             a21 = new Vector3(-f1.y, f1.x, 0),
//             a22 = new Vector3(-f2.y, f2.x, 0);

//         // Test axis a00
//         p0 = Vector3.Dot(v0, a00);
//         p1 = Vector3.Dot(v1, a00);
//         p2 = Vector3.Dot(v2, a00);
//         r = extents.y * Mathf.Abs(f0.z) + extents.z * Mathf.Abs(f0.y);

//         if (Mathf.Max(-Mathf.Max(p0, p1, p2), Mathf.Min(p0, p1, p2)) > r)
//         {
//             return false;
//         }

//         // Test axis a01
//         p0 = Vector3.Dot(v0, a01);
//         p1 = Vector3.Dot(v1, a01);
//         p2 = Vector3.Dot(v2, a01);
//         r = extents.y * Mathf.Abs(f1.z) + extents.z * Mathf.Abs(f1.y);

//         if (Mathf.Max(-Mathf.Max(p0, p1, p2), Mathf.Min(p0, p1, p2)) > r)
//         {
//             return false;
//         }

//         // Test axis a02
//         p0 = Vector3.Dot(v0, a02);
//         p1 = Vector3.Dot(v1, a02);
//         p2 = Vector3.Dot(v2, a02);
//         r = extents.y * Mathf.Abs(f2.z) + extents.z * Mathf.Abs(f2.y);

//         if (Mathf.Max(-Mathf.Max(p0, p1, p2), Mathf.Min(p0, p1, p2)) > r)
//         {
//             return false;
//         }

//         // Test axis a10
//         p0 = Vector3.Dot(v0, a10);
//         p1 = Vector3.Dot(v1, a10);
//         p2 = Vector3.Dot(v2, a10);
//         r = extents.x * Mathf.Abs(f0.z) + extents.z * Mathf.Abs(f0.x);
//         if (Mathf.Max(-Mathf.Max(p0, p1, p2), Mathf.Min(p0, p1, p2)) > r)
//         {
//             return false;
//         }

//         // Test axis a11
//         p0 = Vector3.Dot(v0, a11);
//         p1 = Vector3.Dot(v1, a11);
//         p2 = Vector3.Dot(v2, a11);
//         r = extents.x * Mathf.Abs(f1.z) + extents.z * Mathf.Abs(f1.x);

//         if (Mathf.Max(-Mathf.Max(p0, p1, p2), Mathf.Min(p0, p1, p2)) > r)
//         {
//             return false;
//         }

//         // Test axis a12
//         p0 = Vector3.Dot(v0, a12);
//         p1 = Vector3.Dot(v1, a12);
//         p2 = Vector3.Dot(v2, a12);
//         r = extents.x * Mathf.Abs(f2.z) + extents.z * Mathf.Abs(f2.x);

//         if (Mathf.Max(-Mathf.Max(p0, p1, p2), Mathf.Min(p0, p1, p2)) > r)
//         {
//             return false;
//         }

//         // Test axis a20
//         p0 = Vector3.Dot(v0, a20);
//         p1 = Vector3.Dot(v1, a20);
//         p2 = Vector3.Dot(v2, a20);
//         r = extents.x * Mathf.Abs(f0.y) + extents.y * Mathf.Abs(f0.x);

//         if (Mathf.Max(-Mathf.Max(p0, p1, p2), Mathf.Min(p0, p1, p2)) > r)
//         {
//             return false;
//         }

//         // Test axis a21
//         p0 = Vector3.Dot(v0, a21);
//         p1 = Vector3.Dot(v1, a21);
//         p2 = Vector3.Dot(v2, a21);
//         r = extents.x * Mathf.Abs(f1.y) + extents.y * Mathf.Abs(f1.x);

//         if (Mathf.Max(-Mathf.Max(p0, p1, p2), Mathf.Min(p0, p1, p2)) > r)
//         {
//             return false;
//         }

//         // Test axis a22
//         p0 = Vector3.Dot(v0, a22);
//         p1 = Vector3.Dot(v1, a22);
//         p2 = Vector3.Dot(v2, a22);
//         r = extents.x * Mathf.Abs(f2.y) + extents.y * Mathf.Abs(f2.x);

//         if (Mathf.Max(-Mathf.Max(p0, p1, p2), Mathf.Min(p0, p1, p2)) > r)
//         {
//             return false;
//         }

//         if (Mathf.Max(v0.x, v1.x, v2.x) < -extents.x || Mathf.Min(v0.x, v1.x, v2.x) > extents.x)
//         {
//             return false;
//         }

//         if (Mathf.Max(v0.y, v1.y, v2.y) < -extents.y || Mathf.Min(v0.y, v1.y, v2.y) > extents.y)
//         {
//             return false;
//         }

//         if (Mathf.Max(v0.z, v1.z, v2.z) < -extents.z || Mathf.Min(v0.z, v1.z, v2.z) > extents.z)
//         {
//             return false;
//         }

//         var normal = Vector3.Cross(f1, f0).normalized;
//         var pl = new Plane(normal, Vector3.Dot(normal, tri.a));
//         return Intersects(pl, aabb);
//     }

//     public static bool Intersects(Plane pl, Bounds aabb)
//     {
//         Vector3 center = aabb.center;
//         var extents = aabb.max - center;

//         var r = extents.x * Mathf.Abs(pl.normal.x) + extents.y * Mathf.Abs(pl.normal.y) + extents.z * Mathf.Abs(pl.normal.z);
//         var s = Vector3.Dot(pl.normal, center) - pl.distance;

//         return Mathf.Abs(s) <= r;
//     }
// }
