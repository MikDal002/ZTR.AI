using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using ZTR.AI.Algorithms.Core;
using ZTR.AI.Common.Core.RandomEngines;
using MathSharp;

namespace ZTR.AI.SimulatedAnnealing.Core.Tests;

public class SimulatedAnnealingEngineTests
{
    [Test]
    public void SimulatedAnnealingEngine_ShouldBeConstructibleWithDefaultArguments()
    {
        var sa = new SimulatedAnnealingEngine(_ => 0.0, 1, Vector<double>.Build.Dense(1, Double.MaxValue), Vector<double>.Build.Dense(1, Double.MinValue));
        sa.Should().NotBeNull();
    }

    [Test]
    public void SimmualtedAnnealingEngine_TakesLowerEnergyAsResultAlways()
    {
        var proposedResult = 100.0;
        var sa = new SimulatedAnnealingEngine(_ => proposedResult > 0 ? --proposedResult : 0, 100, Vector<double>.Build.Dense(1, Double.MaxValue), Vector<double>.Build.Dense(1, Double.MinValue));
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
        Action action = () => new SimulatedAnnealingEngine(_ => 0, 0, Vector<double>.Build.Dense(1, Double.MaxValue), Vector<double>.Build.Dense(1, Double.MinValue), endingTemperature: temperature);
        action.Should().Throw<ArgumentOutOfRangeException>();
#pragma warning restore CA1806 // Do not ignore method results
    }

    [Test]
    public void SimulatedAnnealingEngine_TakesHigherEnergyWhenProbabilityHappens()
    {
        var randomEngine = new ConstRandomEngine(0.0);
        var proposedResult = 0.0;
        var sa = new SimulatedAnnealingEngine(_ => proposedResult == 100.0 ? ++proposedResult : 100, 100, Vector<double>.Build.Dense(1, Double.MaxValue), Vector<double>.Build.Dense(1, Double.MinValue), endingTemperature: 0.1, randomEngine: randomEngine);
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
        var sa = new SimulatedAnnealingEngine(function, 10,  Vector<double>.Build.Dense(2, Double.MinValue), Vector<double>.Build.Dense(2, Double.MaxValue), endingTemperature: 0.001);
        while (!sa.IsFinished)
        {
            sa.NextStep();
        }

        sa.Result.Should().BeApproximately(resultShouldBe, maxResultMiss);
    }

    private static readonly IEnumerable<object> SimpleFunctionsWithErrorMargin = TestFunctions.SimpleFunctionsWithErrorMargin;

    [Test]
    [Ignore("Because it's remainder for the")]
    public void SimulatedAnnealing_ShouldAutomaticallyCalculateProposedTemperature()
    {
        // Tutaj opis jak dobieraæ wartoœci https://youtu.be/gX-X85dCib0?t=1226
        // Slajd 15
        // P = 0,98
        // f(i) - f(j)(\delta f)
        // T_0 = - (\delta f) / ln(P)
    }
}