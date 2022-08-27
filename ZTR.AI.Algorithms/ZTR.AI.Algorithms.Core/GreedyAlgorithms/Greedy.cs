using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZTR.AI.Common.Core.RandomEngines;
using ZTR.AI.SimulatedAnnealing.Core;

namespace ZTR.AI.Algorithms.Core.GreedyAlgorithms;

public class GreedyEngineForSingleDimensional
{
    private readonly IPositionProvider _provider;
    public Func<double, double> FunctionToOptimize { get; }
    public double MinimumSolutionRange { get; }
    public double MaximumSolutionRange { get; }
    public bool IsFinished { get; private set; }
    public double Result { get; private set; } = Double.PositiveInfinity;
    public double CurrentResult { get; private set; }

    public GreedyEngineForSingleDimensional(Func<double, double> functionToOptimize, double minimumSolutionRange = double.NegativeInfinity, 
        double maximumSolutionRange = double.PositiveInfinity, IPositionProvider? provider = null, IRandomEngine? engine = null)
    {
        _provider = provider ?? new TemperatureKeepAndDownPositionProvider(100, 0.1, engine);
        FunctionToOptimize = functionToOptimize;
        MinimumSolutionRange = minimumSolutionRange;
        MaximumSolutionRange = maximumSolutionRange;
    }

    public void NextStep()
    {
        if (IsFinished) return;
        var proposedPosition = _provider.GetNextPosition( CurrentResult, MaximumSolutionRange, MinimumSolutionRange);
        var proposedValue = FunctionToOptimize(proposedPosition);
        if (proposedValue < Result) Result = proposedValue;
        IsFinished = _provider.IsFinished();
    }
}
