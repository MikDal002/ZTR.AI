using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Light.GuardClauses;
using ZTR.AI.Algorithms.Core.PositionProviders;
using ZTR.AI.Common.Core.RandomEngines;
using ZTR.AI.SimulatedAnnealing.Core;

namespace ZTR.AI.Algorithms.Core;

public class GreedyEngineForSingleDimensional
{
    private readonly IPositionProvider _provider;
    private long _stepsToFinish;

    public double CurrentSolution { get; private set; }
    public Func<double, double> FunctionToOptimize { get; }
    public double MinimumSolutionRange { get; }
    public double MaximumSolutionRange { get; }
    public double Result { get; private set; }
    public bool IsFinished => _stepsToFinish <= 0;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="functionToOptimize"></param>
    /// <param name="minimumSolutionRange"></param>
    /// <param name="maximumSolutionRange"></param>
    /// <param name="stepsToFinish">Steps to finish defines how long greedy algorithms will search for the best solution. It is decreased after every test of new solution.</param>
    /// <param name="provider"></param>
    /// <param name="engine">Will be used to generate starting position and will be provided to new Position Provider if it is null.</param>
    public GreedyEngineForSingleDimensional(Func<double, double> functionToOptimize, double minimumSolutionRange = double.NegativeInfinity,
        double maximumSolutionRange = double.PositiveInfinity, long stepsToFinish = 1_000, IPositionProvider? provider = null, IRandomEngine? engine = null)
    {
        _stepsToFinish = stepsToFinish.MustBeGreaterThan(0);

        engine ??= new SystemRandomEngine();
        _provider = provider ?? new TemperatureKeepAndDownPositionProvider(100, 0.1, engine);

        FunctionToOptimize = functionToOptimize;
        MinimumSolutionRange = minimumSolutionRange;
        MaximumSolutionRange = maximumSolutionRange;

        CurrentSolution = engine.NextDoubleFromRange(MinimumSolutionRange, MaximumSolutionRange);
        Result = FunctionToOptimize(CurrentSolution);
    }

    public void NextStep()
    {
        if (IsFinished) return;

        var proposedPosition = _provider.GetNextPosition(CurrentSolution, MaximumSolutionRange, MinimumSolutionRange);
        var proposedValue = FunctionToOptimize(proposedPosition);

        if (proposedValue < Result)
        {
            Result = proposedValue;
            CurrentSolution = proposedPosition;
        }

        _stepsToFinish -= 1;
    }
}

