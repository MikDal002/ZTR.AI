using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Extensions;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Fitnesses;

namespace ZTR.AI.Algorithms.Core.GeneticAlgorithm.Solutions.MyProblem
{
    public class XYGenericChromosomeFitness : IFitness<SimpleChromosome>
    {
        public Func<Vector<double>, double> _function;

        public XYGenericChromosomeFitness(Func<Vector<double>, double> function)
        {
            _function = function;
        }

        public double Evaluate(SimpleChromosome chromosome)
        {
            if (chromosome.Genes.Count != 64) throw new Exception();

            var values = new[]
            {
                (double)BitConverter.ToSingle(chromosome.Genes.Take(32).ToBytes()),
                (double)BitConverter.ToSingle(chromosome.Genes.Skip(32).ToBytes()),
            };

            return _function(Vector<double>.Build.Dense(values));
        }
    }
}