using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Light.GuardClauses;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Chromosomes;

namespace ZTR.AI.Algorithms.Core.GeneticAlgorithm.Selections;

public class RouletteSelection : ISelection
{
    private readonly Random _random = new();
    public bool? IsReversed { get; set; } = null;

    /// <inheritdoc />
    public IEnumerable<T> SelectChromosomes<T>(Generation<T> previousGeneration, int requiredNumberOfParents)
        where T : IChromosome
    {
        var min = previousGeneration.Min();
        if (IsReversed == null)
        {
            var max = previousGeneration.Max();
            max.MustNotBeNullReference();

            IsReversed = max!.Fitness < min!.Fitness;
            HasMinus = min.Fitness < 0 || max.Fitness < 0;
        }
        else if (!HasMinus) HasMinus = previousGeneration.Any(d => d.Fitness!.Value < 0);


        var sumOfFitnesse = 0.0;

        foreach (var chrom in previousGeneration)
        {
            if (!chrom.Fitness.HasValue)
                throw new ArgumentException("Chromosome doesn't have fitness calculated!");

            sumOfFitnesse += GetFitness(chrom.Fitness.Value, min!.Fitness!.Value);
        }


        var parentThresholds = new List<double>(requiredNumberOfParents);

        for (var i = 0; i < requiredNumberOfParents; ++i)
            parentThresholds.Add(_random.NextDouble() * sumOfFitnesse);

        parentThresholds = parentThresholds.OrderBy(d => d).ToList();

        var minimumParentThreshold = parentThresholds[0];

        var selectionProgress = 0.0;
        foreach (var chrom in previousGeneration)
        {
            selectionProgress += GetFitness(chrom.Fitness!.Value, min.Fitness!.Value);
            if (minimumParentThreshold > selectionProgress) continue;

            parentThresholds.RemoveAt(0);
            yield return chrom;

            if (parentThresholds.Count == 0) yield break;
            minimumParentThreshold = parentThresholds[0];
        }

        if (parentThresholds.Count == 0) Debug.WriteLine("Nie wszystko się odstrzeliło...");
    }

    public bool HasMinus { get; set; }

    private double GetFitness(double fitness, double currentMin = double.NaN)
    {
        if (IsReversed!.Value && HasMinus)
        {
            if (double.IsNaN(currentMin)) throw new ArgumentException(nameof(currentMin));
            var retValue = -fitness + Math.Abs(currentMin);
            if (retValue < 0) Debug.WriteLine("Coś nie bangla");
            return retValue;
        }

        if (IsReversed!.Value)
            return 1.0 / fitness;
        return fitness;
    }
}