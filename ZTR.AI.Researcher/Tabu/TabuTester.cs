using MathNet.Numerics.LinearAlgebra;
using Microsoft.Extensions.Logging;
using ZT.AI.Researcher;
using ZTR.AI.Algorithms.Core;

namespace ZTR.AI.Researcher.Tabu;

public class TabuTester : BaseTester<TabuOptions>, ITester<TabuOptions>
{
    private readonly TestFunctionProvider _testFunctionProvider;

    public TabuTester(TestFunctionProvider testFunctionProvider, ILogger<TabuOptions> logger, RawResultsHandler handler) : base(logger, handler)
    {
        _testFunctionProvider = testFunctionProvider;
    }

    public override (double Result, int Steps) RunInternal(TabuOptions options, int stepsToDo)
    {

        var (function, min, max) = _testFunctionProvider.GetFunction(options.TestFunction);
        var greedyEngine = new TabuEngine(function, min, max,
            Vector<double>.Build.Dense(2, options.TabuRange),
            options.RestartsAmount, options.ExplorationAmount, options.TabuListMax);
        int steps = 0;
        while (!greedyEngine.IsFinished && steps < stepsToDo)
        {
            greedyEngine.NextStep();
            ++steps;
        }

        return (greedyEngine.Result, steps);
    }
}