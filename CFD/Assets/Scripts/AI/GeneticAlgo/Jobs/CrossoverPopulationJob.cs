using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace CFD.GA
{
    [BurstCompile]
    public struct CrossoverPopulationJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<VoxelStructure> parent1;
        [ReadOnly] public NativeArray<VoxelStructure> parent2;
        [ReadOnly] public NativeArray<VoxelAction> allActions;
        public NativeArray<VoxelStructure> offspring;
        public NativeArray<VoxelAction> newActions;
        public NativeArray<int> actionOffsets;
        public void Execute(int index)
        {
            int startIdx = actionOffsets[index];
            int maxActions = Mathf.Max(parent1[index].actionCount, parent2[index].actionCount);
            for (int i = 0; i < maxActions; i++)
            {
                newActions[startIdx + i] = (UnityEngine.Random.value < 0.5f)
                    ? allActions[parent1[index].startIndex + i]
                    : allActions[parent2[index].startIndex + i];
            }
            offspring[index] = new VoxelStructure { startIndex = startIdx, actionCount = maxActions };
        }
    }
}