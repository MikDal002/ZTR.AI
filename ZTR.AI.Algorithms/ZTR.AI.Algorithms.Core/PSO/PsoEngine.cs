using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using Light.GuardClauses;
using ZTR.AI.Common.Core.RandomEngines;
using ZTR.AI.Algorithms.Core.PositionProviders;

namespace ZTR.AI.Algorithms.Core.PSO
{
    public class PsoEngine
    {
        private readonly IRandomEngine _randomEngine;

        private readonly IReadOnlyCollection<Particle> _particles;
        private readonly IPositionProvider _provider;

        public PsoEngine(Func<Vector<double>, double> functionToOptimize, Vector<double> minimumSolutionRange, Vector<double> maximumSolutionRange,
            int amountOfParticles = 10, IPositionProvider? provider = null, IRandomEngine? randomEngine = null)
        {
            _randomEngine = randomEngine ?? new SystemRandomEngine();
            _provider = provider ?? new RandomPositionProvider(_randomEngine);

            amountOfParticles.MustBeGreaterThan(0);
            FunctionToOptimize = functionToOptimize;
            MaximumSolutionRange = maximumSolutionRange;
            MinimumSolutionRange = minimumSolutionRange.MustBeTheSameCountAs(MaximumSolutionRange);

            _particles = Enumerable
                .Range(0, amountOfParticles)
                .Select(_ => FormParticle(minimumSolutionRange, maximumSolutionRange, randomEngine))
                .ToList();
        }

        private Particle FormParticle(Vector<double> minimumSolutionRange, Vector<double> maximumSolutionRange, IRandomEngine? randomEngine)
        {
            var position = _provider.GetNextPosition(Vector<double>.Build.Dense(minimumSolutionRange.Count, 0),
                maximumSolutionRange, minimumSolutionRange);
            var velocityArray = new[] {_randomEngine.NextDouble() * 0.1, _randomEngine.NextDouble() * 0.1};
            var velocity = Vector<double>.Build.Dense(velocityArray);
            return new Particle(FunctionToOptimize, position, velocity, _randomEngine, minimumSolutionRange, maximumSolutionRange);
        }


        private int currentNumber { get; set; } = 0;
        public void NextStep()
        {
            var newGeneration = false;
            ;
            
            if (currentNumber >= _particles.Count)
            {
                currentNumber = 0;
                newGeneration = true;
            }

            var currentParticle = _particles.ElementAt(currentNumber++);
            if (!currentParticle.WasInited)
            {
                currentParticle.Init();
                return;
            }

            if (newGeneration)
            {
                var bestParticle = _particles.MinBy(d => d.BestKnownValue);
                if (bestParticle.BestKnownValue < Result)
                {
                    BestKnownPosition = bestParticle.Position;
                    Result = bestParticle.BestKnownValue;
                }
            }

            currentParticle.MakeAStep(BestKnownPosition);
        }


        public double GetCurrentBestSolution()
        {
            return _particles.Min(d => d.BestKnownValue);
        }

        public Vector<double> BestKnownPosition { get; private set; }

        public Vector<double> CurrentSolution { get; private set; }
        public Func<Vector<double>, double> FunctionToOptimize { get; }
        public Vector<double> MinimumSolutionRange { get; }
        public Vector<double> MaximumSolutionRange { get; }
        public double Result { get; private set; } = Double.PositiveInfinity;
        public bool IsFinished { get; }


    }
}
