using System;
using System.Drawing;

namespace ZTR.AI.Algorithms.Core.GeneticAlgorithm.Solutions.KnapsackProblem;

public record Insert
{
    public string Name { get; init; } = null!;
    public double Weight { get; init; }
    public double Profit { get; init; }


}