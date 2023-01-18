using Light.GuardClauses.Exceptions;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using ZT.AI.Researcher;
using ZTR.AI.Algorithms.Core;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Crossovers;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Fitnesses;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Mutations;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Populations;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Selections;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Solutions.MyProblem;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Terminations;

namespace ZTR.AI.Researcher.GA;

public class GenethicsAlgorithmTester : BaseTester<GenethicsAlgorithmsOptions>, ITester<GenethicsAlgorithmsOptions>
{
    private readonly TestFunctionProvider _testFunctionProvider;
    private ILogger<GenethicsAlgorithmsOptions> _logger;
    public GenethicsAlgorithmTester(TestFunctionProvider testFunctionProvider, ILogger<GenethicsAlgorithmsOptions> logger) : base(logger)
    {
        _testFunctionProvider = testFunctionProvider;
        _logger = logger;
    }

    public override (double Result, int Steps) RunInternal(GenethicsAlgorithmsOptions options, int stepsToDo)
    {

        var (function, min, max) = _testFunctionProvider.GetFunction(options.TestFunction);
        var selection = new RouletteSelection();
        var factory = new FloatChromosomeFactory(max: (float)max[0], min: (float)min[0]);

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
        //ga.GenerationHasGone += ((sender, generation) => _logger.LogDebug("Genration has gone!"));
        //_logger.LogInformation("GA running...");
         ga.Start();
         //_logger.LogInformation("Best solution found has {0}.", ga.BestChromosome.Fitness);

         return (ga.BestChromosome!.Fitness!.Value, -1);
    }
}