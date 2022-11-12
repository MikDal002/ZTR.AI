using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using ZTR.AI.Algorithms.Core.PositionProviders;
using ZTR.AI.Common.Core.RandomEngines;

namespace ZTR.AI.SimulatedAnnealing.Core.Tests;

public class TemperatureKeepAndDownPositionProviderTests
{
    [Test]
    public void TemperatureKeepAndDownPositionProvider_HasTheSameWorkingTemperatureAsStartingTemperature_AtTheBegining([Range(0.1, 100.0, 1)] double temperature)
    {
        var sa = new TemperatureKeepAndDownPositionProvider(temperature);
        sa.WorkingTemperature.Should().Be(temperature);
    }

    [Test]
    public void TemperatureKeepAndDownPositionProvider_DecreasesTemperatureOnEveryStep_UntilFinish([Range(-5, 5, 1)] double currentSolution, [Range(-5, 5, 1)] double min, [Range(-5, 5, 1)] double max)
    {
        var cut = new TemperatureKeepAndDownPositionProvider(100);
        while (!cut.IsFinished)
        {
            cut.GetNextPosition(Vector.Build.Dense(1, currentSolution), Vector.Build.Dense(1, max), Vector.Build.Dense(1, min));
        }

        cut.WorkingTemperature.Should().BeLessOrEqualTo(0.1);
    }
}

public class SimulatedAnnealingEngineTests
{
    [Test]
    public void SimulatedAnnealingEngine_ShouldBeConstructibleWithDefaultArguments()
    {
        var sa = new SimulatedAnnealingEngine(_ => 0.0, 1, Vector.Build.Dense(1, Double.MaxValue), Vector.Build.Dense(1, Double.MinValue));
        sa.Should().NotBeNull();
    }

    [Test]
    public void SimmualtedAnnealingEngine_TakesLowerEnergyAsResultAlways()
    {
        var proposedResult = 100.0;
        var sa = new SimulatedAnnealingEngine(_ => proposedResult > 0 ? --proposedResult : 0, 100, Vector.Build.Dense(1, Double.MaxValue), Vector.Build.Dense(1, Double.MinValue));
        while (!sa.IsFinished)
        {
            sa.NextStep();
        }

        sa.Result.Should().Be(0.0);
    }

    [TestCase(0.0)]
    [TestCase(-0.0)]
    [TestCase(-1.0)]
    public void SimmualtedAnnealingEngine_TemperatureMustBeHigherThanZero(double temperature)
    {
#pragma warning disable CA1806 // Do not ignore method results
        Action action = () => new SimulatedAnnealingEngine(_ => 0, 0, Vector.Build.Dense(1, Double.MaxValue), Vector.Build.Dense(1, Double.MinValue), endingTemperature: temperature);
        action.Should().Throw<ArgumentOutOfRangeException>();
#pragma warning restore CA1806 // Do not ignore method results
    }

    [Test]
    public void SimulatedAnnealingEngine_TakesHigherEnergyWhenProbabilityHappens()
    {
        var randomEngine = new ConstRandomEngine(0.0);
        var proposedResult = 0.0;
        var sa = new SimulatedAnnealingEngine(_ => proposedResult == 100.0 ? ++proposedResult : 100, 100, Vector.Build.Dense(1, Double.MaxValue), Vector.Build.Dense(1, Double.MinValue), endingTemperature: 0.1, randomEngine: randomEngine);
        while (!sa.IsFinished)
        {
            sa.NextStep();
        }

        sa.Result.Should().Be(100.0);
    }

    [TestCaseSource(nameof(SimpleFunctionsWithErrorMargin))]
    [Repeat(5)]
    public void SimulatedAnnealingEngine_ForSimpleFunction_ShouldTakePossibleMaximum(Func<Vector<double>, double> function, double resultShouldBe, double maxResultMiss)
    {
        var sa = new SimulatedAnnealingEngine(function, 100,  Vector.Build.Dense(1, Double.MaxValue), Vector.Build.Dense(1, Double.MinValue), endingTemperature: 0.01);
        while (!sa.IsFinished)
        {
            sa.NextStep();
        }

        sa.Result.Should().BeApproximately(resultShouldBe, maxResultMiss);
    }

    private static readonly IEnumerable<object> SimpleFunctionsWithErrorMargin = TestFunctions.SimpleFunctionsWithErrorMargin;

    [Test]
    [Ignore("Because it's remainder for the")]
    public void SimulatedAnnealing_ShouldAutomaticallyCalculatePropsedTemperature()
    {
        // Tutaj opis jak dobieraæ wartoœci https://youtu.be/gX-X85dCib0?t=1226
        // Slajd 15
        // P = 0,98
        // f(i) - f(j)(\delta f)
        // T_0 = - (\delta f) / ln(P)
    }
}

public static class TestFunctions
{

    public static readonly IEnumerable<object> SimpleFunctionsWithErrorMargin = new List<object>()
    {
        new object[] {new Func<Vector<double>, double>(x =>
        {
            if (x.Any(d => d <= 0)) return 0;
            else return x.Sum();
        }), 0, 0.1 },
        //new object[] {new Func<Vector<double>, double>(x => Math.Pow(x, 2.0) ), 0, 0.2 },

        //// Non-Convex Unimodal Functions https://machinelearningmastery.com/1d-test-functions-for-function-optimization/
        //new object[] {new Func<Vector<double>, double>(x => (-(x + Math.Sin(x)) * Math.Exp(-Math.Pow(x, 2.0)))), -0.82423939785504219, 1 },
        //new object[] {new Func<Vector<double>, double>(Math.Sin), -1, 0.1 },
        //new object[] {new Func<Vector<double>, double>(Math.Cos), -1, 0.1 },
    };
}