using Light.GuardClauses.Exceptions;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using ZT.AI.Researcher;
using ZTR.AI.Algorithms.Core;
using ZTR.AI.Algorithms.Core.PSO;

namespace ZTR.AI.Researcher.PSO;

public class ParticleSwarmOptimisationAlgorithmsTester : BaseTester<ParticleSwarmOptimisationAlgorithmsOptions>, ITester<ParticleSwarmOptimisationAlgorithmsOptions>
{
    private readonly TestFunctionProvider _testFunctionProvider;
    private ILogger<ParticleSwarmOptimisationAlgorithmsOptions> _logger;
    public ParticleSwarmOptimisationAlgorithmsTester(TestFunctionProvider testFunctionProvider, ILogger<ParticleSwarmOptimisationAlgorithmsOptions> logger) : base(logger)
    {
        _testFunctionProvider = testFunctionProvider;
        _logger = logger;
    }
    
    public override (double Result, int Steps) RunInternal(ParticleSwarmOptimisationAlgorithmsOptions options, int stepsToDo)
    {

        var (function, min, max) = _testFunctionProvider.GetFunction(options.TestFunction);
        var pso = new PsoEngine(function, min, max);
        int steps = 0;
        while (!pso.IsFinished && steps < stepsToDo)
        {
            pso.NextStep();
            ++steps;
        }

        var result = double.IsPositiveInfinity(pso.Result) ? pso.GetCurrentBestSolution() : pso.Result; 

        return (result, steps);
    }
}