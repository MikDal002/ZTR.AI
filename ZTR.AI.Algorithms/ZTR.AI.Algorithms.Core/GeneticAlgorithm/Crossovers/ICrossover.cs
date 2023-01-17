using System.Collections.Generic;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Chromosomes;

namespace ZTR.AI.Algorithms.Core.GeneticAlgorithm.Crossovers;

public interface ICrossover<T> where T : IChromosome
{
    /// <summary>
    ///     Amount of parents needed to crossover
    /// </summary>
    int RequiredNumberOfParents { get; }

    /// <summary>
    ///     Makes children from parents
    /// </summary>
    /// <param name="parents">It's length must be at least equal to RequiredNumberOfParents</param>
    /// <returns></returns>
    IEnumerable<T> MakeChildren(IEnumerable<T> parents);
}