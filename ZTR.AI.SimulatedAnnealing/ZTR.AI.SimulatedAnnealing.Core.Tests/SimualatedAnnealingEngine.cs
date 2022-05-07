using System;

namespace ZTR.AI.SimulatedAnnealing.Core.Tests;

/// <summary>
/// TODO: Dodać kończenie algorytmu wtedy, gdy wartość się już nie zmienia.
/// </summary>
internal class SimualatedAnnealingEngine
{
    private readonly IRandomEngine _random;
    private readonly Func<double, double> _value;

    public SimualatedAnnealingEngine(Func<double, double> value, double temperature,
        double endingTemperature = 0.1, Func<double, double>? temperatureDecreaser = null, IRandomEngine? randomEngine = null)
    {
        if (temperature <= 0) throw new ArgumentOutOfRangeException(nameof(temperature));
        _value = value;
        WorkingTemperature = StartingTemperature = temperature;
        EndingTemperature = endingTemperature;
        TemperatureDecreaser = temperatureDecreaser ?? (temp => 0.95*temp) ;
        _random = randomEngine ?? RandomEngine.GetDefault();
        CurrentSolution = 0.0;
    }

    public double StartingTemperature { get; }

    public double CurrentSolution { get; private set; }
    public double WorkingTemperature { get; private set; }
    public double EndingTemperature { get; }
    public Func<double, double> TemperatureDecreaser { get; }
    public bool IsFinished { get; private set; }
    public double Result { get; private set; } = double.PositiveInfinity;

    public void NextStep()
    {
        if (WorkingTemperature <= 0.0 || IsFinished)
        {
            IsFinished = true;
            return;
        }
        
        // TODO: TO jest chłodzenie schodkowe, tak, że każda z temperatur jest utrzymywana przez pewną chwilę.
        //       Niemniej, użytkownik powinien mieć możliwość wyboru schładzania wykładniczego oraz piłowatego (jak piła) i innych.
        for (int i = 0; i < StartingTemperature / WorkingTemperature; i++)
        {
            double proposedPosition = GenerateNewPositionSomewhereNearToCurrentSolution();

            var proposedResult = _value(proposedPosition);
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
        }

        WorkingTemperature = TemperatureDecreaser(WorkingTemperature);
        if (WorkingTemperature <= EndingTemperature) IsFinished = true;
    }

    private double GenerateNewPositionSomewhereNearToCurrentSolution()
    {
        return CurrentSolution + (_random.NextDoubleFromRandomDistribution() * WorkingTemperature);
    }

    private void SetNewResult(double proposedResult, double proposedPosition)
    {
        Result = proposedResult;
        CurrentSolution = proposedPosition;
    }
}