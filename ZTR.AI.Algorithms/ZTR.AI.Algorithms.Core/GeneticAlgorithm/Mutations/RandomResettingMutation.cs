using System;
using System.Linq;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Chromosomes;

namespace ZTR.AI.Algorithms.Core.GeneticAlgorithm.Mutations;

public class RandomResettingMutation<T, E> : BaseMutation<T> where T : IGenableChromosome<E>
{
    private readonly IGenableChromosomeFactory<T, E> _factory;

    public RandomResettingMutation(IGenableChromosomeFactory<T, E> factory)
    {
        _factory = factory;
    }

    protected override T MutateImplementation(T offspring)
    {
        Random rnd = new();
        var genes = offspring.Genes.ToList();
        var geneToMutate = rnd.Next(genes.Count);
        genes[geneToMutate] = _factory.GetGene(geneToMutate);
        return _factory.FromGenes(genes);
    }
}