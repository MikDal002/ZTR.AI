using Microsoft.Extensions.Logging;
using ZT.AI.Researcher;
using ZTR.AI.Algorithms.Core;

namespace ZTR.AI.Researcher.SimulatedAnnealing;

public class SimulatedAnnealingTester : BaseTester<SimulatedAnnealingOptions>, ITester<SimulatedAnnealingOptions>
{
    private readonly TestFunctionProvider _testFunctionProvider;

    public SimulatedAnnealingTester(TestFunctionProvider testFunctionProvider, ILogger<SimulatedAnnealingOptions> logger, RawResultsHandler handler) : base(logger, handler)
    {
        _testFunctionProvider = testFunctionProvider;
    }

    public override (double Result, int Steps) RunInternal(SimulatedAnnealingOptions options, int stepsToDo)
    {

        var (function, min, max) = _testFunctionProvider.GetFunction(options.TestFunction);
        var simulatedAnnealingEngine = new SimulatedAnnealingEngine(function, options.StartingTemperature, min, max);
        int steps = 0;
        while (!simulatedAnnealingEngine.IsFinished && steps < stepsToDo)
        {
            simulatedAnnealingEngine.NextStep();
            ++steps;
        }

        return (simulatedAnnealingEngine.Result, steps);
    }
}