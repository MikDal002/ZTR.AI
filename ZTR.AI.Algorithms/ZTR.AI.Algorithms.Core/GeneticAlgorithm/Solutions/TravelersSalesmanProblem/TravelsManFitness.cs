using System;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Fitnesses;

namespace ZTR.AI.Algorithms.Core.GeneticAlgorithm.Solutions.TravelersSalesmanProblem;

public class TravelsManFitness : IFitness<TravelerProblemChromosome>
{
    /// <inheritdoc />
    public double Evaluate(TravelerProblemChromosome chromosome)
    {
        var cities = chromosome.Genes;
        var sum = 0.0;
        for (var i = 0; i < cities.Count - 1; i++)
        {
            var first = cities[i].Location;
            var second = cities[i + 1].Location;
            sum += Math.Sqrt(Math.Pow(first.X - second.X, 2) + Math.Pow(first.Y - second.Y, 2));
        }

        chromosome.TotalPath = sum;

        return sum;
    }
}