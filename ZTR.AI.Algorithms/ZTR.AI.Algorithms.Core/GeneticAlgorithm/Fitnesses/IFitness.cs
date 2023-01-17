using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Chromosomes;

namespace ZTR.AI.Algorithms.Core.GeneticAlgorithm.Fitnesses;

public interface IFitness<T> where T : IChromosome
{
    double Evaluate(T chromosome);
}