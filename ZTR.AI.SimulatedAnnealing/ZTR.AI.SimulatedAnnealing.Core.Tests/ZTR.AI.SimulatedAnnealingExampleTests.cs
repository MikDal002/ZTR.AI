using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.FakeItEasy;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using FluentAssertions;
using NUnit.Framework.Internal;

namespace ZTR.AI.SimulatedAnnealing.Core.Tests
{
    public class SimulatedAnnealingTests
    {

        [Test]
        public void ObjectShouldBe_CreatableWithDefaultArguments()
        {
            var sa = new SimulatedAnnealing(_ => 0.0);
            sa.Should().NotBeNull();
        }

        [Test]
        public void SimulatedAnnealing_ReturnsTheSameValue_ForConstantFunction([Range(-10, 10, 1)] double returnValue)
        {
            var sa = new SimulatedAnnealing(w => 1.0d);
            sa.Result.Should().Be(1.0d);
        }
    }
    /// <summary>
    /// Ciekawa prezentacja, która wyjaœnia kilka elementów i ma bogat¹ bibliografiê. 
    /// https://jakubnowosad.com/ahod/11-simulated-annealing.html#14
    ///
    /// Artyku³ na Wiki
    /// https://pl.wikipedia.org/wiki/Symulowane_wy%C5%BCarzanie
    ///
    /// Ciekawa implementacja symulowanego wyrza¿ania dla problemu komiwoja¿era
    /// https://toddwschneider.com/posts/traveling-salesman-with-simulated-annealing-r-and-shiny/
    ///
    /// Szybka pomoc w sprawie zmiany temperatury - zmiana za pomoc¹ mno¿enia a nie odejmowania.
    /// Fakt, ¿e na koniec kroków powinno byæ wiêcej. 
    /// https://jameshfisher.com/2019/05/28/what-is-simulated-annealing/
    /// 
    /// </summary>
    public class SimulatedAnnealingEngineTests
    {
        [Test]
        public void SimulatedAnnealingEngine_ShouldBeConstructibleWithDefaultArguments()
        {
            var sa = new SimualatedAnnealingEngine(_ => 0.0, 1);
            sa.Should().NotBeNull();
        }

        [Test]
        public void SimulatedAnnealingEngine_HasTheSameWorkingTemperatureAsStartingTemperature_AtTheBegining([Range(0.1, 100.0, 1)] double temperature)
        {
            var sa = new SimualatedAnnealingEngine(_ => 0.0, temperature);
            sa.WorkingTemperature.Should().Be(temperature);
        }

        [Test]
        public void SimulatedAnnealingEngine_DecreasesTemperatureOnEveryStep_UntilFinish()
        {
            var sa = new SimualatedAnnealingEngine(_ => 0.0, 100, 0.1);
            while (!sa.IsFinished)
            {
                sa.NextStep();
            }

            sa.WorkingTemperature.Should().BeLessOrEqualTo(0.1);
        }

        [Test]
        public void SimmualtedAnnealingEngine_TakesLowerEnergyAsResultAlways()
        {
            var proposedResult = 100.0;
            var sa = new SimualatedAnnealingEngine(_ => proposedResult > 0 ? --proposedResult : 0, 100);
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
            Action action = () => new SimualatedAnnealingEngine(_ => 0, 0, temperature);
            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Test]
        public void SimulatedAnnealingEngine_TakesHigherEnergyWhenProbabilityHappens()
        {
            var randomEngine = new ConstRandomEngine(0.0);
            var proposedResult = 0.0;
            var sa = new SimualatedAnnealingEngine(_ => proposedResult == 100.0 ?  ++proposedResult : 100, 100, 0.1, randomEngine: randomEngine);
            while (!sa.IsFinished)
            {
                sa.NextStep();
            }

            sa.Result.Should().Be(100.0);
        }

        [TestCaseSource(nameof(SimpleFunctionsWithErrorMaring))]
        [Repeat(5)]
        public void SimulatedAnnealingEngine_ForSimpleFunction_ShouldTakePossibleMaximum(Func<double, double> function, double resultShouldBe, double maxResultMiss)
        {
            var sa = new SimualatedAnnealingEngine(function, 100, 0.01);
            while (!sa.IsFinished)
            {
                sa.NextStep();
            }
            
            sa.Result.Should().BeApproximately(resultShouldBe, maxResultMiss);
        }

        private static IEnumerable<object> SimpleFunctionsWithErrorMaring = new List<object>()
        {
            new object[] {new Func<double, double>(x => x < 0 ? 0 : x), 0, 0.1 },
            new object[] {new Func<double, double>(x => Math.Pow(x, 2.0) ), 0, 0.2 },

            // Non-Convex Unimodal Functions https://machinelearningmastery.com/1d-test-functions-for-function-optimization/
            new object[] {new Func<double, double>(x => (-(x + Math.Sin(x)) * Math.Exp(-Math.Pow(x, 2.0)))), -0.82423939785504219, 0.1 },
            new object[] {new Func<double, double>(Math.Sin), -1, 0.1 },
            new object[] {new Func<double, double>(Math.Cos), -1, 0.1 },
        };

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
}
