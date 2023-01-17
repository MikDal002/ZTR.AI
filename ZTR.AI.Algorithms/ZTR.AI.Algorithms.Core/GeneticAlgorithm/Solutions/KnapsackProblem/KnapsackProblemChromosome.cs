using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Chromosomes;

namespace ZTR.AI.Algorithms.Core.GeneticAlgorithm.Solutions.KnapsackProblem;

public record KnapsackProblemChromosome : FitnessComparableChromosome, IGenableChromosome<Insert>
{
    public double MaxWeight { get; }
    private readonly List<Insert> _genes;

    public double TotalProfit { get; set; }
    public double TotalWeight { get; set; }


    /// <inheritdoc />
    public IReadOnlyList<Insert> Genes => _genes;

    public KnapsackProblemChromosome(double maxWeight, IEnumerable<Insert> cities)
    {
        MaxWeight = maxWeight;
        _genes = cities.ToList();
    }
}