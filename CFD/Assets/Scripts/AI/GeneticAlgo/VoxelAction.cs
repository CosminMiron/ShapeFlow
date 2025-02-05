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

            List<Vector3Int> possibleDirections = new List<Vector3Int>();

            if (!exteriorVoxels.Contains(selectedVoxel + Vector3Int.up))
            {
                possibleDirections.Add(Vector3Int.down);
            }

            if (!exteriorVoxels.Contains(selectedVoxel + Vector3Int.down))
            {
                possibleDirections.Add(Vector3Int.down);
            }

            if (!exteriorVoxels.Contains(selectedVoxel + Vector3Int.forward))
            {
                possibleDirections.Add(Vector3Int.back);
            }

            if (!exteriorVoxels.Contains(selectedVoxel + Vector3Int.back))
            {
                possibleDirections.Add(Vector3Int.forward);
            }

            if (!exteriorVoxels.Contains(selectedVoxel + Vector3Int.right))
            {
                possibleDirections.Add(Vector3Int.left);
            }

            if (!exteriorVoxels.Contains(selectedVoxel + Vector3Int.left))
            {
                possibleDirections.Add(Vector3Int.right);
            }

            // while (possibleDirections.Count == 0)
            // {
            //     if (exteriorVoxels.Contains(selectedVoxel + Vector3Int.forward) || !exteriorVoxels.Contains(selectedVoxel + Vector3Int.forward * -1))
            //     {
            //         selectedVoxel = exteriorVoxels.ElementAt(Random.Range(0, exteriorVoxels.Count));
            //         continue;
            //     }

            //     if (exteriorVoxels.Contains(selectedVoxel + Vector3Int.forward + new Vector3Int(0, 1, 0)) || exteriorVoxels.Contains(selectedVoxel + Vector3Int.forward + new Vector3Int(0, -1, 0)) || exteriorVoxels.Contains(selectedVoxel + Vector3Int.forward + new Vector3Int(1, 0, 0)) || exteriorVoxels.Contains(selectedVoxel + Vector3Int.forward + new Vector3Int(-1, 0, 0)))
            //     {
            //         possibleDirections.Add(Vector3Int.down);
            //     }

            //     possibleDirections.Add(Vector3Int.down);
            // }


            Vector3Int selectedDirection = possibleDirections[Random.Range(0, possibleDirections.Count)];

            return new VoxelAction(selectedVoxel, selectedDirection);
        }
    }
}