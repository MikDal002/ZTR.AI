using FluentAssertions;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using ZTR.AI.Algorithms.Core;

namespace ZTR.AI.SimulatedAnnealing.Core.Tests
{
    public class GreedyEngineForMultiDimensionalTests
    {
        [Test]
        public void METHOD()
        {
            var greedyEngineForMultiDimensional = new GreedyEngine(vector => 0, Vector<double>.Build.Dense(5, -1.0), Vector<double>.Build.Dense(5, 1.0));
            greedyEngineForMultiDimensional.NextStep();
        }
    }

    public class GreedyEngineForSingleDimensionalTests
    {
        [Test]
        public void GreedyEngineForSingleDimensional_ShouldBeConstructibleForDefaultArguments()
        {
            var cut = new GreedyEngine(_ => 0, DenseVector.Create(1, 0), DenseVector.Create(1, 1));
            cut.Should().NotBeNull();
        }

        [Test]
        public void Greedy_AlwaysReturnsTheSameValue_ForConstFunction([Range(-5, 5, 1)] double expectedOutcome)
        {
            var cut = new GreedyEngine(_ => expectedOutcome, DenseVector.Create(1, 0), DenseVector.Create(1, 1));
            while (!cut.IsFinished)
            {
                cut.NextStep();
            }
            cut.Result.Should().Be(expectedOutcome);
        }

        [TestCaseSource(nameof(SimpleFunctionsWithErrorMargin))]
        [Repeat(5)]
        public void Greedy_ForSimpleFunctionWithMinimumCloseToZero_ShouldTakePossibleMaximum(Func<Vector<double>, double> function,
            double resultShouldBe, double maxResultMiss)
        {
            var cut = new GreedyEngine(function, DenseVector.Create(1, -5), DenseVector.Create(1, 5));

            while (!cut.IsFinished)
            {
                cut.NextStep();
            }

            cut.Result.Should().BeApproximately(resultShouldBe, maxResultMiss);
        }

        private static readonly IEnumerable<object> SimpleFunctionsWithErrorMargin = TestFunctions.SimpleFunctionsWithErrorMargin;
    }
}