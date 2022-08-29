using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using ZTR.AI.Algorithms.Core.GreedyAlgorithms;

namespace ZTR.AI.SimulatedAnnealing.Core.Tests
{
    public class GreedyEngineForSingleDiemensionalTests
    {
        [Test]
        public void GreedyEngineForSingleDimensional_ShouldBeConstructibleForDefaultArguments()
        {
            var cut = new GreedyEngineForSingleDimensional(_ => 0);
            cut.Should().NotBeNull();
        }

        [Test]
        public void Greedy_AlwaysReturnsTheSameValue_ForConstFunction([Range(-5, 5, 1)] double expectedOutcome)
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
        public void Greedy_ForSimpleFunctionWithMinimumCloseToZero_ShouldTakePossibleMaximum(Func<double, double> function,
            double resultShouldBe, double maxResultMiss)
        {
            var cut = new GreedyEngineForSingleDimensional(function, -5, 5);

            while (!cut.IsFinished)
            {
                cut.NextStep();
            }

            cut.Result.Should().BeApproximately(resultShouldBe, maxResultMiss);
        }

        private static readonly IEnumerable<object> SimpleFunctionsWithErrorMargin = TestFunctions.SimpleFunctionsWithErrorMargin;
    }
}