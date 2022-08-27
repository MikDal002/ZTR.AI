using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using ZTR.AI.Algorithms.Core.GreedyAlgorithms;
using ZTR.AI.Common.Core.RandomEngines;

namespace ZTR.AI.SimulatedAnnealing.Core.Tests;
public class GreedyEngineForSingleDiemensionalTests
{
    [Test]
    public void GreedyEngineForSingleDimensional_ShouldBeConstructibleForDefaultArguments()
    {
        var cut = new GreedyEngineForSingleDimensional(_ => 0);
        cut.Should().NotBeNull();
    }

    [Test]
    public void Test_OfTests([Range(-100, 100.0, 5)] double expectedOutcome)
    {
        var cut = new GreedyEngineForSingleDimensional(_ => expectedOutcome);
        while (!cut.IsFinished)
        {
            cut.NextStep();
        }
        cut.Result.Should().Be(expectedOutcome);
    }

    [TestCaseSource(nameof(SimpleFunctionsWithErrorMargin))]
    [Repeat(5)]
    public void Greedy_ForSimpleFunction_ShouldTakePossibleMaximum(Func<double, double> function,
        double resultShouldBe, double maxResultMiss)
    {
        var cut = new GreedyEngineForSingleDimensional(function);

        while (!cut.IsFinished)
        {
            cut.NextStep();
        }

        cut.Result.Should().BeApproximately(resultShouldBe, maxResultMiss);
    }

    private static readonly IEnumerable<object> SimpleFunctionsWithErrorMargin = TestFunctions.SimpleFunctionsWithErrorMargin;
}


public class SimulatedAnnealingEngineTests
{
    [Test]
    public void SimulatedAnnealingEngine_ShouldBeConstructibleWithDefaultArguments()
    {
        var sa = new SimulatedAnnealingEngine(_ => 0.0, 1);
        sa.Should().NotBeNull();
    }

    [Test]
    public void SimulatedAnnealingEngine_HasTheSameWorkingTemperatureAsStartingTemperature_AtTheBegining([Range(0.1, 100.0, 1)] double temperature)
    {
        var sa = new SimulatedAnnealingEngine(_ => 0.0, temperature);
        sa.PositionProvider.WorkingTemperature.Should().Be(temperature);
    }

    [Test]
    public void SimulatedAnnealingEngine_DecreasesTemperatureOnEveryStep_UntilFinish()
    {
        var sa = new SimulatedAnnealingEngine(_ => 0.0, 100, 0.1);
        while (!sa.IsFinished)
        {
            sa.NextStep();
        }

        sa.PositionProvider.WorkingTemperature.Should().BeLessOrEqualTo(0.1);
    }

    [Test]
    public void SimmualtedAnnealingEngine_TakesLowerEnergyAsResultAlways()
    {
        var proposedResult = 100.0;
        var sa = new SimulatedAnnealingEngine(_ => proposedResult > 0 ? --proposedResult : 0, 100);
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
        Action action = () => new SimulatedAnnealingEngine(_ => 0, 0, temperature);
        action.Should().Throw<ArgumentOutOfRangeException>();
#pragma warning restore CA1806 // Do not ignore method results
    }

    [Test]
    public void SimulatedAnnealingEngine_TakesHigherEnergyWhenProbabilityHappens()
    {
        var randomEngine = new ConstRandomEngine(0.0);
        var proposedResult = 0.0;
        var sa = new SimulatedAnnealingEngine(_ => proposedResult == 100.0 ? ++proposedResult : 100, 100, 0.1, randomEngine: randomEngine);
        while (!sa.IsFinished)
        {
            sa.NextStep();
        }

        sa.Result.Should().Be(100.0);
    }

    [TestCaseSource(nameof(SimpleFunctionsWithErrorMargin))]
    [Repeat(5)]
    public void SimulatedAnnealingEngine_ForSimpleFunction_ShouldTakePossibleMaximum(Func<double, double> function, double resultShouldBe, double maxResultMiss)
    {
        var sa = new SimulatedAnnealingEngine(function, 100, 0.01);
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
        new object[] {new Func<double, double>(x => x < 0 ? 0 : x), 0, 0.1 },
        new object[] {new Func<double, double>(x => Math.Pow(x, 2.0) ), 0, 0.2 },

        // Non-Convex Unimodal Functions https://machinelearningmastery.com/1d-test-functions-for-function-optimization/
        new object[] {new Func<double, double>(x => (-(x + Math.Sin(x)) * Math.Exp(-Math.Pow(x, 2.0)))), -0.82423939785504219, 1 },
        new object[] {new Func<double, double>(Math.Sin), -1, 0.1 },
        new object[] {new Func<double, double>(Math.Cos), -1, 0.1 },
    };
}