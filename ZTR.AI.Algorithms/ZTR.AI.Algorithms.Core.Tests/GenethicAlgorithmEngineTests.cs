using System;
using System.Collections.Generic;
using FluentAssertions;
using MathNet.Numerics.LinearAlgebra;
using NUnit.Framework;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Crossovers;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Mutations;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Populations;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Selections;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Solutions.MyProblem;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Terminations;

namespace ZTR.AI.SimulatedAnnealing.Core.Tests
{
    public class GenethicAlgorithmEngineTests
    {
        private static readonly IEnumerable<object> SimpleFunctionsWithErrorMargin = TestFunctions.SimpleFunctionsWithErrorMargin;

        [TestCaseSource(nameof(SimpleFunctionsWithErrorMargin))]
        [Repeat(5)]
        public void GenethicAlgorithmEngine_ForSimpleFunction_ShouldTakePossibleMaximum(Func<Vector<double>, double> function, double resultShouldBe, double maxResultMiss)
        {
            var selection = new RouletteSelection();
            var factory = new FloatChromosomeFactory(max: (float)double.MaxValue, min: (float)double.MinValue);

            Func<FloatChromosomeFactory, ICrossover<SimpleChromosome>> uniformCrossoverFactory
                = (factory) => new UniformCrossover<SimpleChromosome, bool>(factory, 0.1);
            Func<int, double, FloatChromosomeFactory, IMutation<SimpleChromosome>> mutationFactory
                = (swaps, mutationThreshold, factory) => new SwapMutation<SimpleChromosome, bool>(factory) { MutationThreshold = mutationThreshold, AmountOfSwaps = swaps };
            Func<ITermination> terminationFactory = () => new GenerationNumberTermination(10); // () => new TheSameNeuronWinner(100)
            var populationNumber = 10;

            var fitness = new XYGenericChromosomeFitness(function);

            var crossover = uniformCrossoverFactory(factory);
            var mutation = mutationFactory(1, 0.9, factory);

            var population = new Population<SimpleChromosome>(populationNumber, populationNumber * 2, factory, crossover,
                mutation, selection);
            var ga = new GeneticAlgorithm<SimpleChromosome>(population, fitness);
            ga.Termination = terminationFactory();
        
        
            foreach (var result in ga.Start())
            {
            
            }
            //_logger.LogInformation("Best solution found has {0}.", ga.BestChromosome.Fitness);

            ga.BestChromosome!.Fitness!.Value.Should().BeApproximately(resultShouldBe, maxResultMiss);
        }

    }
}