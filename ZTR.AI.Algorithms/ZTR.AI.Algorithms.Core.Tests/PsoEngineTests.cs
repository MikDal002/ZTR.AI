using System;
using System.Collections.Generic;
using FluentAssertions;
using MathNet.Numerics.LinearAlgebra;
using NUnit.Framework;
using ZTR.AI.Algorithms.Core.PSO;

namespace ZTR.AI.SimulatedAnnealing.Core.Tests
{
    public class PsoEngineTests
    {
        private static readonly IEnumerable<object> SimpleFunctionsWithErrorMargin = TestFunctions.SimpleFunctionsWithErrorMargin;

        [TestCaseSource(nameof(SimpleFunctionsWithErrorMargin))]
        [Repeat(5)]
        public void PsoEngine_ForSimpleFunction_ShouldTakePossibleMaximum(Func<Vector<double>, double> function, double resultShouldBe, double maxResultMiss)
        {
            var sa = new PsoEngine(function, Vector<double>.Build.Dense(2, Double.MinValue), Vector<double>.Build.Dense(2, Double.MaxValue), amountOfGenerations: 10);
            while (!sa.IsFinished)
            {
                sa.NextStep();
            }

            sa.Result.Should().BeApproximately(resultShouldBe, maxResultMiss);
        }

    }
}