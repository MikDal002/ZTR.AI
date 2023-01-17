using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Chromosomes;

#nullable enable
namespace ZTR.AI.Algorithms.Core.GeneticAlgorithm.Mutations;

public interface IMutation<T> where T : IChromosome
{
    T? Mutate(T offspring);
}