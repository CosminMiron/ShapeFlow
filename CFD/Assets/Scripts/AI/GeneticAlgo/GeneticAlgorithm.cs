using System.Collections.Generic;
using UnityEngine;

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
            _voxelizedMesh.Voxelize();

            _voxelizedData = _voxelizedMesh.VoxelizedData;
            VoxelGrid.Instance.Init(_voxelizedData.Hash);

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
            var initialDragCoeficient = _voxelizedData.CalculateDragCoefficient(_voxelizedMesh.forward, _voxelizedMesh.Speed, _voxelizedMesh.AirDensity);
            UnityEngine.Debug.LogError($"Initial drag coeficient: {initialDragCoeficient}");
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

            EvaluateFitness();

            var bestFitness = float.MaxValue;
            VoxelStructure bestStructure = null;

            foreach (var structure in population)
            {
                if (structure.Fitness < bestFitness)
                {
                    bestStructure = structure;
                    bestFitness = bestStructure.Fitness;
                }
            }

            UnityEngine.Debug.LogError($"Best fitness drag coeficient: {bestFitness}");
            var ceva = _voxelizedData.GetVoxelizedMutation(bestStructure.actions);
            //_voxelizedMesh.GridPoints = new List<Vector3Int>(ceva.GridPoints);

            UnityEngine.Debug.LogError(bestStructure.ToString());

            ShowDiff(ceva.GridPoints);
        }

        private void EvaluateFitness()
        {
            foreach (var structure in population)
            {
                //float drag = _voxelizedData.CalculateObjectDragForce(_voxelizedMesh.forward,_voxelizedMesh.Speed, _voxelizedMesh.AirDensity);
                //float surfaceArea = _voxelizedData.CalculateFrontalArea();
                //structure.Fitness = 1f / (drag + surfaceArea + 1f); // Example fitness function
                var ceva = _voxelizedData.GetVoxelizedMutation(structure.actions);
                structure.Fitness = ceva.CalculateDragCoefficient(_voxelizedMesh.forward, _voxelizedMesh.Speed, _voxelizedMesh.AirDensity);
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

        private void ShowDiff(List<Vector3Int> newList)
        {
            var points = new List<Vector3Int>();
            foreach (var element in newList)
            {
                if (_voxelizedMesh.GridPoints.Contains(element)) continue;

                points.Add(element);
            }

            foreach (var element in _voxelizedMesh.GridPoints)
            {
                if (!newList.Contains(element))
                {
                    points.Add(element);
                }
            }

            _voxelizedMesh.GridPoints = points;
        }
    }
}