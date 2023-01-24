using System;
using MathNet.Numerics.LinearAlgebra;
using ZTR.AI.Common.Core.RandomEngines;

namespace ZTR.AI.Algorithms.Core.PSO
{
    class Particle
    {
        private readonly Func<Vector<double>, double> _functionToOptimize;
        private readonly IRandomEngine _randomEngine;
        private readonly Vector<double> _minimumSolutionRange;
        private readonly Vector<double> _maximumSolutionRange;
        private readonly double _slowingFactor;
        private readonly double _localPositionFactor;
        private readonly double _globalPositionFactor;

        public Particle(Func<Vector<double>, double> functionToOptimize, Vector<double> startingPosition, 
            Vector<double> velocity, 
            IRandomEngine randomEngine, Vector<double> minimumSolutionRange, Vector<double> maximumSolutionRange,
            double slowingFactor = 0.8, double localPositionFactor = 0.1, double globalPositionFactor = 0.1)
        {
            _slowingFactor = slowingFactor;
            _localPositionFactor = localPositionFactor;
            _globalPositionFactor = globalPositionFactor;
            _functionToOptimize = functionToOptimize;
            this._randomEngine = randomEngine;
            _minimumSolutionRange = minimumSolutionRange;
            _maximumSolutionRange = maximumSolutionRange;
            Position = startingPosition;
            Velocity = velocity;
        }

        public Vector<double> BestKnownPositionByParticle { get; private set; }
        public double BestKnownValue { get; private set; } = double.PositiveInfinity;
        public Vector<double> Position { get; private set; }
        public Vector<double> Velocity { get; private set; }

        public  bool WasInited { get; private set; }
        public void Init()
        {
            CalculateNewPosition();
            WasInited = true;
        }

        public void MakeAStep(Vector<double> bestGlobalKnownPosition)
        {
            if (!WasInited) throw new Exception("You must init particle first!");

            var localRandomModifier = _randomEngine.NextDouble();
            var globalRandomModifier = _randomEngine.NextDouble();

            Velocity = _slowingFactor * Velocity 
                       + localRandomModifier * _localPositionFactor * (BestKnownPositionByParticle - Position)
                       + globalRandomModifier * _globalPositionFactor + (bestGlobalKnownPosition - Position);

            CalculateNewPosition();
        }

        private void CalculateNewPosition()
        {
            var currentSolution = _functionToOptimize(Position);

            if (currentSolution < BestKnownValue)
            {
                BestKnownValue = currentSolution;
                BestKnownPositionByParticle = Position;
            }

            Position += Velocity;

            var positionCount = Position.Count;
            
            for (int i = 0; i < positionCount; i++)
            {
                if (Position[i] < _minimumSolutionRange[i]) Position[i] = _minimumSolutionRange[i];
                if (Position[i] > _maximumSolutionRange[i]) Position[i] = _maximumSolutionRange[i];
            }
        }
    }
}