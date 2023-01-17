using System;
using System.Diagnostics;
using System.Linq;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Extensions;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Fitnesses;

namespace ZTR.AI.Algorithms.Core.GeneticAlgorithm.Solutions.MyProblem;

/// <summary>
///     https://www.sfu.ca/~ssurjano/mccorm.html
/// </summary>
public class MCCORMICKChromosomeFitness : IFitness<SimpleChromosome>
{
    /// <inheritdoc />
    public double Evaluate(SimpleChromosome chromosome)
    {
        if (chromosome.Genes.Count != 64) throw new Exception();


        var val1 = BitConverter.ToSingle(chromosome.Genes.Take(32).ToBytes());
        var val2 = BitConverter.ToSingle(chromosome.Genes.Skip(32).Take(32).ToBytes());
        var result = Math.Sin(val1 + val2) + Math.Pow(val1 - val2, 2) - 1.5 * val1 + 2.5 * val2 + 1;
        if (double.IsNaN(result))
        {
            Debug.WriteLine("No i problem, bo wyszło NaN...");
            return double.PositiveInfinity;
        }

        if (result < -2) Debug.WriteLine("hohoho!");

        return result;
    }
}