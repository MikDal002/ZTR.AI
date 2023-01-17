using System;
using System.Collections.Generic;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Chromosomes;

namespace ZTR.AI.Algorithms.Core.GeneticAlgorithm.Solutions.MyProblem;

public record SimpleChromosome : FitnessComparableChromosome, IGenableChromosome<bool>
{
    public SimpleChromosome(List<bool> genes)
    {
        Genes = genes;
    }

    public IReadOnlyList<bool> Genes { get; }


    /// <inheritdoc />
    public override int CompareTo(object? obj)
    {
        return -base.CompareTo(obj);
    }
}