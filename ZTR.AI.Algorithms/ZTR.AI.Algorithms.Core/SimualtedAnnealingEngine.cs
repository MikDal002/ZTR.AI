using System;
using ZTR.AI.Common.Core.RandomEngines;

namespace ZTR.AI.SimulatedAnnealing.Core;

public interface IPositionProvider
{
    double GetNextPosition(double currentSolution, double maximumSolutionRange, double minimumSolutionRange);
    bool IsFinished();
}

public interface ITemperatureBasedPositionProvider : IPositionProvider
{
    double WorkingTemperature { get; }
}

/// <summary>
/// This position provider is designed to be used with simulated annealing. It is
/// working to start at the specified temperature, and then slowly go down keeping
/// lower temperature longer and longer. Temperature determines how drasticlly can new position differ then the previous one
///
/// T ^
/// E | -
/// M |  --
/// P |    ---
/// E |       ----
/// R |           -----
/// A |                ------
/// T |                     --------
/// U |
/// R |
/// E |
///   +------------------------------->
///         T I M E  
/// 
/// </summary>
public class TemperatureKeepAndDownPositionProvider : ITemperatureBasedPositionProvider
{
    private readonly IRandomEngine _randomEngine;
    private int _inTemperatureValue;

    public TemperatureKeepAndDownPositionProvider(double temperature, double endingTemperature,
        IRandomEngine? randomEngine = null, Func<double, double>? temperatureDecreaser = null)
    {
        _randomEngine = randomEngine ?? new SystemRandomEngine();
        WorkingTemperature = StartingTemperature = temperature;
        EndingTemperature = endingTemperature;
        TemperatureDecreaser = temperatureDecreaser ?? (temp => 0.95 * temp);
        _inTemperatureValue = 0;

    }
    public double StartingTemperature { get; }
    public double EndingTemperature { get; }
    public Func<double, double> TemperatureDecreaser { get; }
    public double WorkingTemperature { get; private set; }

    public bool IsFinished()
    {
        return WorkingTemperature <= EndingTemperature;
    }

    public double GetNextPosition(double currentSolution, double maximumSolutionRange, double minimumSolutionRange)
    {
        var completelyRandomSolution = currentSolution + (_randomEngine.NextDoubleFromRandomDistribution() * WorkingTemperature);

        if (_inTemperatureValue > StartingTemperature / WorkingTemperature)
        {
            WorkingTemperature = TemperatureDecreaser(WorkingTemperature);
            _inTemperatureValue = 0;
        }
        else
        {
            _inTemperatureValue++;
        }

        if (completelyRandomSolution < minimumSolutionRange)
        {
            return minimumSolutionRange;
        }

        if (completelyRandomSolution > maximumSolutionRange)
        {
            return maximumSolutionRange;
        }

        return completelyRandomSolution;
    }
}

/// /// <summary>
/// Ciekawa prezentacja, która wyjaśnia kilka elementów i ma bogatą bibliografię. 
/// https://jakubnowosad.com/ahod/11-simulated-annealing.html#14
///
/// Artykuł na Wiki
/// https://pl.wikipedia.org/wiki/Symulowane_wy%C5%BCarzanie
///
/// Ciekawa implementacja symulowanego wyrzażania dla problemu komiwojażera
/// https://toddwschneider.com/posts/traveling-salesman-with-simulated-annealing-r-and-shiny/
///
/// Szybka pomoc w sprawie zmiany temperatury - zmiana za pomocą mnożenia a nie odejmowania.
/// Fakt, że na koniec kroków powinno być więcej. 
/// https://jameshfisher.com/2019/05/28/what-is-simulated-annealing/
///
/// TODO: Dodać kończenie algorytmu wtedy, gdy wartość się już nie zmienia.
/// 
/// 
/// </summary>
public class SimulatedAnnealingEngine
{
    private readonly IRandomEngine _random;
    private readonly Func<double, double> _functionToOptimize;

    public SimulatedAnnealingEngine(Func<double, double> functionToOptimize, double temperature,
        double endingTemperature = 0.1, Func<double, double>? temperatureDecreaser = null,
        double minimumSolutionRange = double.NegativeInfinity, double maximumSolutionRange = double.PositiveInfinity,
        IRandomEngine? randomEngine = null)
    {
        if (temperature <= 0) throw new ArgumentOutOfRangeException(nameof(temperature));
        _functionToOptimize = functionToOptimize;
        MinimumSolutionRange = minimumSolutionRange;
        MaximumSolutionRange = maximumSolutionRange;
        _random = randomEngine ?? RandomEngine.Default;
        CurrentSolution = 0.0;
        Result = double.PositiveInfinity;
        PositionProvider = new TemperatureKeepAndDownPositionProvider(temperature, endingTemperature, _random, temperatureDecreaser);
    }

    public ITemperatureBasedPositionProvider PositionProvider { get; }
    public double CurrentSolution { get; private set; }
    public double MinimumSolutionRange { get; }
    public double MaximumSolutionRange { get; }
    public bool IsFinished { get; private set; }
    public double Result { get; private set; }
    public void NextStep()
    {
        if (IsFinished || PositionProvider.IsFinished())
        {
            IsFinished = true;
            return;
        }

        // TODO: TO jest chłodzenie schodkowe, tak, że każda z temperatur jest utrzymywana przez pewną chwilę.
        //       Niemniej, użytkownik powinien mieć możliwość wyboru schładzania wykładniczego oraz piłowatego (jak piła) i innych.
        double proposedPosition = PositionProvider.GetNextPosition(CurrentSolution, MaximumSolutionRange, MinimumSolutionRange);

        var proposedResult = _functionToOptimize(proposedPosition);
        if (proposedResult <= Result)
        {
            SetNewResult(proposedResult, proposedPosition);
        }
        else
        {
            var probabilityToTake = _random.NextDouble();
            var maxProbability = Math.Exp((Result - proposedResult) / PositionProvider.WorkingTemperature);

            if (probabilityToTake < maxProbability) SetNewResult(proposedResult, proposedPosition);
        }
    }

    private void SetNewResult(double proposedResult, double proposedPosition)
    {
        Result = proposedResult;
        CurrentSolution = proposedPosition;
    }
}