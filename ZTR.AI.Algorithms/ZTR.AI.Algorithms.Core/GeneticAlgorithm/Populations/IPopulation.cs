using ZTR.AI.Algorithms.Core.GeneticAlgorithm;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Chromosomes;

namespace ZTR.AI.Algorithms.Core.GeneticAlgorithm.Populations;

public interface IPopulation<T> where T : IChromosome
{
    Generation<T> StartNewGeneration();
}