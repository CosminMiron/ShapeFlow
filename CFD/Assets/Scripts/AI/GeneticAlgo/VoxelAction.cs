using UnityEngine;

namespace CFD.GAS
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public struct VoxelAction
    {
        public Vector3Int position; // Where the action happens

        public Vector3Int direction; // In which direction we extend/shrink

        public VoxelAction(Vector3Int pos, Vector3Int dir)
        {
            position = pos;
            direction = dir;
        }

        public static VoxelAction GenerateValidAction(HashSet<Vector3Int> exteriorVoxels)
        {

            if (exteriorVoxels.Count == 0)
            {
                throw new System.Exception("No exterior voxels available!");
            }

            Vector3Int selectedVoxel = exteriorVoxels.ElementAt(Random.Range(0, exteriorVoxels.Count));

            Vector3Int[] possibleDirections = { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right, Vector3Int.forward, Vector3Int.back };

            Vector3Int selectedDirection = possibleDirections[Random.Range(0, possibleDirections.Length)];

            return new VoxelAction(selectedVoxel, selectedDirection);
        }
    }
}