using System.Linq;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Chromosomes;

namespace ZTR.AI.Algorithms.Core.GeneticAlgorithm.Crossovers;

public class SinglePointCrossover<T, E> : MultiPointCrossover<T, E> where T : IGenableChromosome<E>
{
    public SinglePointCrossover(IGenableChromosomeFactory<T, E> factory) : base(1, factory)
    {
    }

    public SinglePointCrossover(int point, IGenableChromosomeFactory<T, E> factory) :
        base(new[] { point }, factory)
    { }
}