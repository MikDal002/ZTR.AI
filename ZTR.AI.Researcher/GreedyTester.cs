using Microsoft.Extensions.Logging;
using ZTR.AI.Algorithms.Core;

namespace ZT.AI.Researcher
{
    public class GreedyTester : BaseTester<GreedyOptions>, ITester<GreedyOptions>
    {
        private readonly TestFunctionProvider _testFunctionProvider;

        public GreedyTester(TestFunctionProvider testFunctionProvider, ILogger<GreedyOptions> logger) : base(logger)
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
}