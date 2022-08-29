using System;
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
        private readonly Func<double, double> _functionToOptimize;

        public SimulatedAnnealingEngine(Func<double, double> functionToOptimize, double temperature,
            double endingTemperature = 0.1, Func<double, double>? temperatureDecreaser = null,
            double minimumSolutionRange = double.NegativeInfinity, double maximumSolutionRange = double.PositiveInfinity,
            IRandomEngine? randomEngine = null)
        {
            if (temperature <= 0) throw new ArgumentOutOfRangeException(nameof(temperature));
            _functionToOptimize = functionToOptimize;
            MinimumSolutionRange = minimumSolutionRange;
            MaximumSolutionRange = maximumSolutionRange;
            _random = randomEngine ?? RandomEngine.Default;
            CurrentSolution = 0.0;
            Result = double.PositiveInfinity;
            PositionProvider = new TemperatureKeepAndDownPositionProvider(temperature, endingTemperature, _random, temperatureDecreaser);
        }

        public ITemperatureBasedPositionProvider PositionProvider { get; }
        public double CurrentSolution { get; private set; }
        public double MinimumSolutionRange { get; }
        public double MaximumSolutionRange { get; }
        public bool IsFinished => PositionProvider.IsFinished;
        public double Result { get; private set; }
        public void NextStep()
        {
            if (IsFinished) return;

                double proposedPosition = PositionProvider.GetNextPosition(CurrentSolution, MaximumSolutionRange, MinimumSolutionRange);

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

        private void SetNewResult(double proposedResult, double proposedPosition)
        {
            Result = proposedResult;
            CurrentSolution = proposedPosition;
        }
    }
}