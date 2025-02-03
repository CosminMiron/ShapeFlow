using System.Collections.Generic;
using UnityEngine;

namespace CFD.GAS
{
    public class VoxelStructure
    {
        public List<VoxelAction> actions;
        public float Fitness;

        public VoxelStructure(int initialActionCount, HashSet<Vector3Int> exteriorVoxels)
        {
            actions = new List<VoxelAction>();
            for (int i = 0; i < initialActionCount; i++)
            {
                actions.Add(VoxelAction.GenerateValidAction(exteriorVoxels));
            }

            Fitness = 0f;
        }
        public VoxelStructure(List<VoxelAction> newActions)
        {
            actions = new List<VoxelAction>(newActions);
            Fitness= 0f;
        }
        public VoxelStructure Copy()
        {
            var copy = new VoxelStructure(new List<VoxelAction>(actions));
            copy.Fitness= Fitness;
            return copy;
        }
    }
}