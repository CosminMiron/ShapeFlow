using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace CFD.GA
{
    public class GeneticAlgorithm : MonoBehaviour
    {
        public int populationSize = 50;
        public float mutationRate = 0.1f;
        public int generations = 100;
        public int sizeX = 32, sizeY = 32, sizeZ = 32;
        public int maxActionsPerIndividual;
        private NativeArray<VoxelStructure> offspring;
        private NativeArray<float> fitnessValues;
        private VoxelGrid voxelGrid;
        private void Start()
        {
            InitializePopulation();
            RunEvolution();
        }
        private NativeArray<VoxelStructure> population;
        private NativeArray<VoxelAction> allActions; // Stores ALL actions
        private NativeArray<int> actionOffsets; // Start indices per individual
        private void InitializePopulation()
        {
            population = new NativeArray<VoxelStructure>(populationSize, Allocator.Persistent);
            allActions = new NativeArray<VoxelAction>(populationSize * maxActionsPerIndividual, Allocator.Persistent);
            actionOffsets = new NativeArray<int>(populationSize, Allocator.Persistent);
            int actionIndex = 0;
            for (int i = 0; i < populationSize; i++)
            {
                population[i] = new VoxelStructure { startIndex = actionIndex, actionCount = Random.Range(5, maxActionsPerIndividual) };
                actionOffsets[i] = actionIndex;
                for (int j = 0; j < population[i].actionCount; j++)
                {
                    allActions[actionIndex++] = VoxelAction.GenerateValidAction(voxelGrid.GetExteriorVoxels());
                }
            }
        }
        private void RunEvolution()
        {
            for (int gen = 0; gen < generations; gen++)
            {
                EvaluateFitness();
                NativeArray<VoxelStructure> newPopulation = new NativeArray<VoxelStructure>(populationSize, Allocator.TempJob);
                CrossoverPopulationJob crossoverJob = new CrossoverPopulationJob
                {
                    parent1 = population,
                    parent2 = population,
                    offspring = newPopulation
                };
                crossoverJob.Schedule(populationSize, 8).Complete();
                MutatePopulationJob mutateJob = new MutatePopulationJob
                {
                    population = newPopulation,
                    exteriorVoxels = voxelGrid.GetExteriorVoxels(),
                    mutationRate = mutationRate
                };
                mutateJob.Schedule(populationSize, 8).Complete();
                population.CopyFrom(newPopulation);
                newPopulation.Dispose();
                Debug.Log($"Generation {gen + 1} completed.");
            }
        }
        private void EvaluateFitness()
        {
            for (int i = 0; i < populationSize; i++)
            {
                fitnessValues[i] = CalculateFitness(population[i]);
            }
        }
        private float CalculateFitness(VoxelStructure structure)
        {
            // Evaluate based on drag coefficient and surface area (to be implemented)
            return Random.Range(0f, 1f); // Placeholder for real simulation-based fitness
        }
        private void OnDestroy()
        {
            population.Dispose();
            offspring.Dispose();
            fitnessValues.Dispose();
            voxelGrid.Dispose();
        }
    }
}
