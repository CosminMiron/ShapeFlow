using System.Collections.Generic;
using UnityEngine;

namespace CFD.GAS
{
    public class VoxelGrid : MonoBehaviour
    {
        public static VoxelGrid Instance { get; private set; }
        private HashSet<Vector3Int> activeVoxels = new HashSet<Vector3Int>();
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }
        /*public void ApplyVoxelActions(List<VoxelAction> actions)
        {
            foreach (var action in actions)
            {
                if (action.isExtend)
                    AddVoxel(action.position + action.direction);
                else
                    RemoveVoxel(action.position);
            }
        }*/
        public void AddVoxel(Vector3Int position)
        {
            activeVoxels.Add(position);
        }
        public void RemoveVoxel(Vector3Int position)
        {
            activeVoxels.Remove(position);
        }
        public bool IsVoxelPresent(Vector3Int position)
        {
            return activeVoxels.Contains(position);
        }
        public HashSet<Vector3Int> GetExteriorVoxels()
        {
            HashSet<Vector3Int> exteriorVoxels = new HashSet<Vector3Int>();
            foreach (var voxel in activeVoxels)
            {
                if (IsExterior(voxel))
                    exteriorVoxels.Add(voxel);
            }
            return exteriorVoxels;
        }
        private bool IsExterior(Vector3Int position)
        {
            Vector3Int[] neighbors =
            {
           Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right, Vector3Int.forward, Vector3Int.back
       };
            foreach (var offset in neighbors)
            {
                if (!activeVoxels.Contains(position + offset))
                    return true; // If any side is exposed, it's an exterior voxel
            }
            return false;
        }
    }
}