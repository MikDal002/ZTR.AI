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
    private double _currentPosition;
    private readonly IPositionProvider _provider;
    public Func<double, double> FunctionToOptimize { get; }
    public double MinimumSolutionRange { get; }
    public double MaximumSolutionRange { get; }
    public bool IsFinished => _provider.IsFinished;
    public double Result { get; private set; }


    public GreedyEngineForSingleDimensional(Func<double, double> functionToOptimize, double minimumSolutionRange = double.NegativeInfinity, 
        double maximumSolutionRange = double.PositiveInfinity, IPositionProvider? provider = null, IRandomEngine? engine = null)
    {
        engine ??= new SystemRandomEngine();
        _provider = provider ?? new TemperatureKeepAndDownPositionProvider(100, 0.1, engine);
        FunctionToOptimize = functionToOptimize;
        MinimumSolutionRange = minimumSolutionRange;
        MaximumSolutionRange = maximumSolutionRange;
        
        _currentPosition = engine.NextDoubleFromRange(MinimumSolutionRange, MaximumSolutionRange);
        Result = FunctionToOptimize(_currentPosition);
    }

    public void NextStep()
    {
        if (IsFinished) return;
        var proposedPosition = _provider.GetNextPosition(_currentPosition, MaximumSolutionRange, MinimumSolutionRange);
        var proposedValue = FunctionToOptimize(proposedPosition);
        if (proposedValue < Result)
        {
            Result = proposedValue;
            _currentPosition = proposedPosition;
        }
    }
}
