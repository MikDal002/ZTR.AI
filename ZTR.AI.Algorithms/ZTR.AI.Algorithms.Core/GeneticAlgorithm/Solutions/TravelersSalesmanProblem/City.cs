using System;
using System.Drawing;

namespace ZTR.AI.Algorithms.Core.GeneticAlgorithm.Solutions.TravelersSalesmanProblem;

public record City
{
    public string Name { get; set; } = null!;
    public PointF Location { get; set; }
}