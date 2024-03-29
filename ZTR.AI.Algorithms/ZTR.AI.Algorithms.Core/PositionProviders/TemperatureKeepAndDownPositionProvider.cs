﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;
using Light.GuardClauses;
using MathNet.Numerics.LinearAlgebra;
using ZTR.AI.Common.Core.RandomEngines;

namespace ZTR.AI.Algorithms.Core.PositionProviders;

/// <summary>
/// This position provider is designed to be used with simulated annealing. It is
/// working to start at the specified temperature, and then slowly go down keeping
/// lower temperature longer and longer. Temperature determines how drasticAlly new position differS then the previous one.
///
/// Longevity of every level of temperature is taken from division: StartingTemperature/WorkingTemperature.
///
/// T ^
/// E | -
/// M |  -
/// P |    --
/// E |       --
/// R |           ---
/// A |                ----
/// T |                      ------
/// U |
/// R |
/// E |
///   +------------------------------->
///         T I M E  
///
/// TODO: TO jest chłodzenie schodkowe, tak, że każda z temperatur jest utrzymywana przez pewną chwilę. Niemniej, użytkownik powinien mieć możliwość wyboru schładzania wykładniczego oraz piłowatego (jak piła) i innych.
/// </summary>
public class TemperatureKeepAndDownPositionProvider : ITemperatureBasedPositionProvider
{
    private readonly IRandomEngine _randomEngine;
    private int _inTemperatureValue;

    public TemperatureKeepAndDownPositionProvider(double temperature, double endingTemperature = 0.1,
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

    public bool IsFinished => WorkingTemperature <= EndingTemperature;

    public Vector<double> GetNextPosition([NotNull] Vector<double> currentSolution, [NotNull] Vector<double> maximumSolutionRange, [NotNull] Vector<double> minimumSolutionRange)
    {
        minimumSolutionRange.MustNotBeNull();
        maximumSolutionRange.MustNotBeNull();
        currentSolution.MustNotBeNull();

        var completelyRandomSolution = currentSolution + (_randomEngine.NextDoubleFromMinusOneToOne() * WorkingTemperature);

        if (_inTemperatureValue > StartingTemperature / WorkingTemperature)
        {
            WorkingTemperature = TemperatureDecreaser(WorkingTemperature);
            _inTemperatureValue = 0;
        }
        else
        {
            _inTemperatureValue++;
        }

        for (int i = 0; i < completelyRandomSolution.Count; ++i)
        {
            if (completelyRandomSolution[i] < minimumSolutionRange[i])
            {
                completelyRandomSolution[i] = minimumSolutionRange[i];
            }

            if (completelyRandomSolution[i] > maximumSolutionRange[i])
            {
                completelyRandomSolution[i] = maximumSolutionRange[i];
            }
        }

        return completelyRandomSolution;
    }
}