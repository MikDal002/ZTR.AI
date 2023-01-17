using System;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Fitnesses;

namespace ZTR.AI.Algorithms.Core.GeneticAlgorithm.Solutions.KnapsackProblem;

public class KnapsackFitness : IFitness<KnapsackProblemChromosome>
{
    /// <inheritdoc />
    public double Evaluate(KnapsackProblemChromosome chromosome)
    {
        var weight = 0.0;
        var profit = 0.0;

        foreach (var insert in chromosome.Genes)
        {
            if (weight + insert.Weight < chromosome.MaxWeight)
            {
                weight += insert.Weight;
                profit += insert.Profit;
            }
            else { break; }
        }

        chromosome.TotalWeight = weight;
        chromosome.TotalProfit = profit;

        return profit;
    }
}