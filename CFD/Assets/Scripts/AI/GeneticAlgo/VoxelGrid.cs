using Unity.Collections;
using UnityEngine;

namespace CFD.GA
{
    public class VoxelGrid
    {
        private NativeArray<int> grid;
        private int sizeX, sizeY, sizeZ;
        public VoxelGrid(int sizeX, int sizeY, int sizeZ)
        {
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            this.sizeZ = sizeZ;
            grid = new NativeArray<int>(sizeX * sizeY * sizeZ, Allocator.Persistent);
        }
        public int GetIndex(int x, int y, int z)
        {
            return x + y * sizeX + z * sizeX * sizeY;
        }
        public NativeHashSet<Vector3Int> GetExteriorVoxels()
        {
            NativeHashSet<Vector3Int> exteriorVoxels = new NativeHashSet<Vector3Int>(0, Allocator.TempJob);
            Vector3Int[] directions = { Vector3Int.right, Vector3Int.left, Vector3Int.up, Vector3Int.down, Vector3Int.forward, Vector3Int.back };
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    for (int z = 0; z < sizeZ; z++)
                    {
                        int index = GetIndex(x, y, z);
                        if (grid[index] == 1)
                        {
                            foreach (var dir in directions)
                            {
                                int nx = x + dir.x, ny = y + dir.y, nz = z + dir.z;
                                if (nx < 0 || ny < 0 || nz < 0 || nx >= sizeX || ny >= sizeY || nz >= sizeZ ||
                                    grid[GetIndex(nx, ny, nz)] == 0)
                                {
                                    exteriorVoxels.Add(new Vector3Int(x, y, z));
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return exteriorVoxels;
        }
        public void Dispose()
        {
            if (grid.IsCreated) grid.Dispose();
        }
    }
}
