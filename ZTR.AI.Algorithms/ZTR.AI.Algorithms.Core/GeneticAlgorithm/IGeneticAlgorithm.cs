using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Chromosomes;

namespace ZTR.AI.Algorithms.Core.GeneticAlgorithm;

public interface IGeneticAlgorithm
{
    public int GenerationsNumber { get; }
    IChromosome BestChromosome { get; }
}