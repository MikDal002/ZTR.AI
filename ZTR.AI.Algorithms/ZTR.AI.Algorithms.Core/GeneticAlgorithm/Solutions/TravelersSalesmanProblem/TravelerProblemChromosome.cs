using System;
using System.Collections.Generic;
using System.Linq;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Chromosomes;

namespace ZTR.AI.Algorithms.Core.GeneticAlgorithm.Solutions.TravelersSalesmanProblem;

public record TravelerProblemChromosome : FitnessComparableChromosome, IGenableChromosome<City>
{
    private readonly List<City> _genes;

    public double TotalPath { get; set; }

    /// <inheritdoc />
    public IReadOnlyList<City> Genes => _genes;

    public TravelerProblemChromosome(IEnumerable<City> cities)
    {
        _genes = cities.ToList();
    }

    /// <inheritdoc />
    public virtual int CompareTo(object? obj)
    {
        return -base.CompareTo(obj);
    }
}