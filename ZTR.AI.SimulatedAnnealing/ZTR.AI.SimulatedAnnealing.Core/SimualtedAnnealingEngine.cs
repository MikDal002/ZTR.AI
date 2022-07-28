using System;
using System.Transactions;

namespace ZTR.AI.SimulatedAnnealing.Core;

/// <summary>
/// TODO: Dodać kończenie algorytmu wtedy, gdy wartość się już nie zmienia.
/// TODO: Make internal!
/// </summary>
public class SimualatedAnnealingEngine
{
    private readonly IRandomEngine _random;
    private readonly Func<double, double> _functionToOptimize;

    public SimualatedAnnealingEngine(Func<double, double> functionToOptimize, double temperature,
        double endingTemperature = 0.1, Func<double, double>? temperatureDecreaser = null,
        double minimumSolutionRange = Double.NegativeInfinity, double maximumSolutionRange = Double.PositiveInfinity,
        IRandomEngine? randomEngine = null)
    {
        if (temperature <= 0) throw new ArgumentOutOfRangeException(nameof(temperature));
        _functionToOptimize = functionToOptimize;
        WorkingTemperature = StartingTemperature = temperature;
        EndingTemperature = endingTemperature;
        MinimumSolutionRange = minimumSolutionRange;
        MaximumSolutionRange = maximumSolutionRange;
        TemperatureDecreaser = temperatureDecreaser ?? (temp => 0.95 * temp);
        _random = randomEngine ?? RandomEngine.GetDefault();
        CurrentSolution = 0.0;
    }

    public double StartingTemperature { get; }

    public double CurrentSolution { get; private set; }
    public double WorkingTemperature { get; private set; }
    public double EndingTemperature { get; }
    public double MinimumSolutionRange { get; }
    public double MaximumSolutionRange { get; }
    public Func<double, double> TemperatureDecreaser { get; }
    public bool IsFinished { get; private set; }
    public double Result { get; private set; } = double.PositiveInfinity;
    private int _inTemepratureValue = 0;
    public void NextStep()
    {
        if (WorkingTemperature <= EndingTemperature || IsFinished)
        {
            IsFinished = true;
            return;
        }

        // TODO: TO jest chłodzenie schodkowe, tak, że każda z temperatur jest utrzymywana przez pewną chwilę.
        //       Niemniej, użytkownik powinien mieć możliwość wyboru schładzania wykładniczego oraz piłowatego (jak piła) i innych.
        double proposedPosition = GenerateNewPositionSomewhereNearToCurrentSolution();

        var proposedResult = _functionToOptimize(proposedPosition);
        if (proposedResult <= Result)
        {
            SetNewResult(proposedResult, proposedPosition);
        }
        else
        {
            var probabilityToTake = _random.NextDouble();
            var maxProbability = Math.Exp((Result - proposedResult) / WorkingTemperature);

            if (probabilityToTake < maxProbability) SetNewResult(proposedResult, proposedPosition);
        }
        
        if (_inTemepratureValue > StartingTemperature / WorkingTemperature)
        {
            WorkingTemperature = TemperatureDecreaser(WorkingTemperature);
            _inTemepratureValue = 0;
        }
        else
        {
            _inTemepratureValue++;
        }
    }

    private double GenerateNewPositionSomewhereNearToCurrentSolution()
    {
        var completelyRandomSolution = CurrentSolution + (_random.NextDoubleFromRandomDistribution() * WorkingTemperature);
        if (double.IsInfinity(MaximumSolutionRange) && double.IsInfinity(MinimumSolutionRange)) return completelyRandomSolution;
        
        if (completelyRandomSolution < MinimumSolutionRange)
        {
            return MinimumSolutionRange;
        }

        if (completelyRandomSolution > MaximumSolutionRange)
        {
            return MaximumSolutionRange;
        }

        return completelyRandomSolution;


    }

    private void SetNewResult(double proposedResult, double proposedPosition)
    {
        Result = proposedResult;
        CurrentSolution = proposedPosition;
    }
}