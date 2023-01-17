using System;
using System.Collections.Generic;
using System.Linq;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Chromosomes;

namespace ZTR.AI.Algorithms.Core.GeneticAlgorithm.Solutions.MyProblem;

public class BitChromosomeFactory : IGenableChromosomeFactory<SimpleChromosome, bool>
{
    private readonly int _length;
    private readonly Random _random;

    public BitChromosomeFactory(int length)
    {
        _length = length;
        _random = new Random();
    }

    /// <inheritdoc />
    public virtual SimpleChromosome CreateNew()
    {
        return new SimpleChromosome(Enumerable.Range(0, _length).Select(d => GetGene(d)).ToList());
    }

    /// <inheritdoc />
    public virtual SimpleChromosome FromGenes(IList<bool> genes)
    {
        if (genes.Count != _length) throw new Exception();
        return new SimpleChromosome(genes.ToList());
    }

    /// <inheritdoc />
    public virtual bool GetGene(int geneNumber)
    {
        return _random.Next() % 2 == 0;
    }
}