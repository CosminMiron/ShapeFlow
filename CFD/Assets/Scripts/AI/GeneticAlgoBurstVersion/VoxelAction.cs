using Unity.Collections;
using UnityEngine;

namespace CFD.GA
{
    public enum VoxelActionType { Extend, Shrink }

    public struct VoxelAction
    {
        public VoxelActionType type;
        public Vector3Int Direction;
        public float magnitude;
        public Vector3Int position;

        public VoxelAction(VoxelActionType type, Vector3Int position, Vector3Int direction, float magnitude = 1f)
        {
            this.type = type;
            Direction = direction;
            this.magnitude = magnitude;
            this.position = position;
        }

        public static VoxelAction GenerateValidAction(NativeHashSet<Vector3Int> exteriorVoxels)
        {
            if (exteriorVoxels.Count == 0) return default;

            NativeArray<Vector3Int> directions = new NativeArray<Vector3Int>(6, Allocator.Temp);

            directions[0] = Vector3Int.left;
            directions[1] = Vector3Int.right;
            directions[2] = Vector3Int.up;
            directions[3] = Vector3Int.down;
            directions[4] = Vector3Int.forward;
            directions[5] = Vector3Int.back;

            NativeArray<Vector3Int> voxelArray = exteriorVoxels.ToNativeArray(Allocator.Temp);
            Vector3Int chosenVoxel = voxelArray[UnityEngine.Random.Range(0, voxelArray.Length)];
            Vector3Int chosenDirection = directions[UnityEngine.Random.Range(0, directions.Length)];
            voxelArray.Dispose();

            VoxelActionType actionType = UnityEngine.Random.value < 0.5f ? VoxelActionType.Extend : VoxelActionType.Shrink;

            return new VoxelAction(actionType, chosenVoxel, chosenDirection);
        }
    }
}
