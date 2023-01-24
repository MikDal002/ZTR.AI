using FluentAssertions;
using MathNet.Numerics.LinearAlgebra.Double;
using NUnit.Framework;
using ZTR.AI.Algorithms.Core.PositionProviders;

namespace ZTR.AI.SimulatedAnnealing.Core.Tests
{
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
}