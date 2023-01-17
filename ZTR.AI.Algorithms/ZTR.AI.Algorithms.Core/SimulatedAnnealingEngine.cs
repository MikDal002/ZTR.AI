using System;
using System.Linq;
using Light.GuardClauses;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using ZTR.AI.Algorithms.Core;
using ZTR.AI.Algorithms.Core.PositionProviders;
using ZTR.AI.Common.Core.RandomEngines;

namespace ZTR.AI.SimulatedAnnealing.Core
{
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
        private readonly Func<Vector<double>, double> _functionToOptimize;

        public ITemperatureBasedPositionProvider PositionProvider { get; }

        public SimulatedAnnealingEngine(Func<Vector<double>, double> functionToOptimize, double startingTemperature,
            Vector<double> minimumSolutionRange, Vector<double> maximumSolutionRange,
            double endingTemperature = 0.1, Func<double, double>? temperatureDecreaser = null,
            IRandomEngine? randomEngine = null)
        {
            startingTemperature.MustBeGreaterThan(0);
            minimumSolutionRange.MustBeTheSameCountAs(maximumSolutionRange,
                message:
                $"Size of {nameof(maximumSolutionRange)} must be the same as size of {nameof(minimumSolutionRange)}!");
            
            minimumSolutionRange.MustNotBeNull();
            maximumSolutionRange.MustNotBeNull();

            _functionToOptimize = functionToOptimize;
            MinimumSolutionRange = minimumSolutionRange;
            MaximumSolutionRange = maximumSolutionRange;
            _random = randomEngine ?? RandomEngine.Default;
            CurrentSolution = Vector<double>.Build.Dense(minimumSolutionRange.Count, 0.0);

            Result = double.PositiveInfinity;
            PositionProvider = new TemperatureKeepAndDownPositionProvider(startingTemperature, endingTemperature, _random, temperatureDecreaser);
        }

        public Vector<double> CurrentSolution { get; private set; }
        public Vector<double> MinimumSolutionRange { get; }
        public Vector<double> MaximumSolutionRange { get; }
        public bool IsFinished => PositionProvider.IsFinished;
        public double Result { get; private set; }
        
        public void NextStep()
        {
            if (IsFinished) return;

            var proposedPosition = PositionProvider.GetNextPosition(CurrentSolution, MaximumSolutionRange, MinimumSolutionRange);

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

        private void SetNewResult(double proposedResult, Vector<double> proposedPosition)
        {
            Result = proposedResult;
            CurrentSolution = proposedPosition;
        }
    }


}