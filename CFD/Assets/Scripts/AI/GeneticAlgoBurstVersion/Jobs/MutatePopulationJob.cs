using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace CFD.GA
{
    [BurstCompile]
    public struct MutatePopulationJob : IJobParallelFor
    {
        public NativeArray<VoxelStructure> population;
        public NativeArray<VoxelAction> allActions;
        [ReadOnly] public NativeHashSet<Vector3Int> exteriorVoxels;
        [ReadOnly] public float mutationRate;
        public void Execute(int index)
        {
            if (UnityEngine.Random.value < mutationRate && population[index].actionCount > 0)
            {
                int actionIndex = population[index].startIndex + UnityEngine.Random.Range(0, population[index].actionCount);
                allActions[actionIndex] = VoxelAction.GenerateValidAction(exteriorVoxels);
            }
        }
    }
}