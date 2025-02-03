using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace CFD.GA
{
    [BurstCompile]
    public struct UpdateVoxelsJob : IJobParallelFor
    {
        public NativeArray<int> voxelGrid;
        public NativeHashSet<Vector3Int> exteriorVoxels;
        [ReadOnly] public NativeArray<VoxelAction> actions;
        public int sizeX, sizeY;
        public void Execute(int index)
        {
            VoxelAction action = actions[index];
            Vector3Int targetPosition = action.position + action.Direction;
            int voxelIndex = targetPosition.x + targetPosition.y * sizeX + targetPosition.z * sizeX * sizeY;
            if (action.type == VoxelActionType.Extend)
            {
                voxelGrid[voxelIndex] = 1;
                exteriorVoxels.Add(targetPosition);
            }
            else if (action.type == VoxelActionType.Shrink)
            {
                voxelGrid[voxelIndex] = 0;
                exteriorVoxels.Remove(targetPosition);
            }
        }
    }
}