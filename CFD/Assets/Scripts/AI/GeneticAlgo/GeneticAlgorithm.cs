using CFD.GA;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.HID.HID;

namespace CFD.GAS
{
    public class GeneticAlgorithm : MonoBehaviour
    {
        [SerializeField] private VoxelizedMesh _voxelizedMesh;
        public int populationSize = 10;
        public int maxActionsPerIndividual = 20;
        public float mutationRate = 0.1f;
        public int generations = 100;
        private List<VoxelStructure> population;
        private HashSet<Vector3Int> exteriorVoxels;
        private VoxelizedData _voxelizedData;

        [ContextMenu("Start GA")]
        private void StartGA()
        {
            _voxelizedData = _voxelizedMesh.VoxelizedData;
            InitializePopulation();
            RunGeneticAlgorithm();
        }
        private void InitializePopulation()
        {
            // Get exterior voxels from your voxel grid
            exteriorVoxels = VoxelGrid.Instance.GetExteriorVoxels();
            population = new List<VoxelStructure>();
            for (int i = 0; i < populationSize; i++)
            {
                population.Add(new VoxelStructure(Random.Range(5, maxActionsPerIndividual), exteriorVoxels));
            }
        }
        private void RunGeneticAlgorithm()
        {
            for (int gen = 0; gen < generations; gen++)
            {
                EvaluateFitness();
                List<VoxelStructure> newPopulation = new List<VoxelStructure>();
                while (newPopulation.Count < populationSize)
                {
                    VoxelStructure parent1 = SelectParent();
                    VoxelStructure parent2 = SelectParent();
                    VoxelStructure offspring = Crossover(parent1, parent2);
                    Mutate(offspring);
                    newPopulation.Add(offspring);
                }
                population = newPopulation;
            }
        }
        private void EvaluateFitness()
        {
            foreach (var structure in population)
            {
                //float drag = _voxelizedData.CalculateObjectDragForce(_voxelizedMesh.forward,_voxelizedMesh.Speed, _voxelizedMesh.AirDensity);
                //float surfaceArea = _voxelizedData.CalculateFrontalArea();
                //structure.Fitness = 1f / (drag + surfaceArea + 1f); // Example fitness function
                structure.Fitness = _voxelizedData.CalculateDragCoefficient(_voxelizedMesh.forward, _voxelizedMesh.Speed, _voxelizedMesh.AirDensity);
            }
        }
        private VoxelStructure SelectParent()
        {
            // Tournament selection
            int tournamentSize = 10;
            VoxelStructure best = null;
            float bestFitness = float.MaxValue;
            for (int i = 0; i < tournamentSize; i++)
            {
                VoxelStructure candidate = population[Random.Range(0, population.Count)];
                if (candidate.Fitness < bestFitness)
                {
                    best = candidate;
                    bestFitness = candidate.Fitness;
                }
            }
            return best.Copy();
        }
        private VoxelStructure Crossover(VoxelStructure parent1, VoxelStructure parent2)
        {
            List<VoxelAction> childActions = new List<VoxelAction>();
            int maxActions = Mathf.Max(parent1.actions.Count, parent2.actions.Count);
            for (int i = 0; i < maxActions; i++)
            {
                if (i < parent1.actions.Count && i < parent2.actions.Count)
                {
                    childActions.Add(Random.value > 0.5f ? parent1.actions[i] : parent2.actions[i]);
                }
                else if (i < parent1.actions.Count)
                {
                    childActions.Add(parent1.actions[i]);
                }
                else if (i < parent2.actions.Count)
                {
                    childActions.Add(parent2.actions[i]);
                }
            }
            return new VoxelStructure(childActions);
        }
        private void Mutate(VoxelStructure structure)
        {
            for (int i = 0; i < structure.actions.Count; i++)
            {
                if (Random.value < mutationRate)
                {
                    structure.actions[i] = VoxelAction.GenerateValidAction(exteriorVoxels);
                }
            }
        }
    }
}