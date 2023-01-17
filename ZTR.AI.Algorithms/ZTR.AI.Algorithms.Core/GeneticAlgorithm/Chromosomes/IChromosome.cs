using System;

namespace ZTR.AI.Algorithms.Core.GeneticAlgorithm.Chromosomes;

public interface IChromosome : IComparable
{
    double? Fitness { get; set; }
}

#pragma warning disable CA1036 // Przesłoń metody porównywalnych typów
public abstract record FitnessComparableChromosome : IChromosome
#pragma warning restore CA1036 // Przesłoń metody porównywalnych typów
{
    /// <inheritdoc />
    public virtual int CompareTo(object? obj)
    {
        if (obj is IChromosome chrom)
            return Fitness!.Value.CompareTo(chrom.Fitness!.Value);
        throw new ArgumentException("Object must be a chromosome!", nameof(obj));
    }

    /// <inheritdoc />
    public double? Fitness { get; set; }
}