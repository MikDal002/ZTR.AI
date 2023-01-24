using MathNet.Numerics.LinearAlgebra.Complex;
using Microsoft.Extensions.Logging;
using ZT.AI.Researcher;
using ZTR.AI.Algorithms.Core;

namespace ZTR.AI.Researcher.Greedy;

public class GreedyTester : BaseTester<GreedyOptions>, ITester<GreedyOptions>
{
    private readonly TestFunctionProvider _testFunctionProvider;

    public GreedyTester(TestFunctionProvider testFunctionProvider, ILogger<GreedyOptions> logger, RawResultsHandler handler) : base(logger, handler)
    {
        _testFunctionProvider = testFunctionProvider;
    }

    public override (double Result, int Steps) RunInternal(GreedyOptions options, int stepsToDo)
    {

        var (function, min, max) = _testFunctionProvider.GetFunction(options.TestFunction);
        var greedyEngine = new GreedyEngine(function, min, max, stepsToDo);
        int steps = 0;
        while (!greedyEngine.IsFinished)
        {
            greedyEngine.NextStep();
            ++steps;
        }

        return (greedyEngine.Result, steps);
    }
}