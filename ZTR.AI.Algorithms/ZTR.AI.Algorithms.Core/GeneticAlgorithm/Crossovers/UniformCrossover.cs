using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Chromosomes;

namespace ZTR.AI.Algorithms.Core.GeneticAlgorithm.Crossovers;

public class UniformCrossover<T, E> : ICrossover<T> where T : IGenableChromosome<E>
{
    public double Ratio { get; }
    private readonly IGenableChromosomeFactory<T, E> _factory;

    public UniformCrossover(IGenableChromosomeFactory<T, E> factory, double ratio)
    {
        Ratio = ratio;
        _factory = factory;
    }

    /// <inheritdoc />
    public int RequiredNumberOfParents => 2;

    /// <inheritdoc />
    public IEnumerable<T> MakeChildren(IEnumerable<T> parents)
    {
        var list = parents.Take(RequiredNumberOfParents).ToList();
        if (list.Count != RequiredNumberOfParents)
            throw new ArgumentException("The number of parents isn't sufficient", nameof(parents));
        if (list[0].Genes.Count != list[1].Genes.Count)
            throw new ArgumentException("Different size of genes is not supported here!");

        var maxCount = list[0].Genes.Count;
        var childGenes1 = new List<E>();
        var childGenes2 = new List<E>();
        double firstParent = 0;
        double secondParent = 0;
        for (var i = 0; i < maxCount; ++i)
        {
            var selectFirst = secondParent + firstParent == 0
                              || firstParent / (secondParent + firstParent) < Ratio;
            if (selectFirst) firstParent++;
            else secondParent++;
            childGenes2.Add(selectFirst ? list[0].Genes[i] : list[1].Genes[i]);
            childGenes1.Add(!selectFirst ? list[0].Genes[i] : list[1].Genes[i]);
        }

        yield return _factory.FromGenes(childGenes1);
        yield return _factory.FromGenes(childGenes2);
    }
}