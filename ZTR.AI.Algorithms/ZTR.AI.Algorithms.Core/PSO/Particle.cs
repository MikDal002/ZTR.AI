using System;
using MathNet.Numerics.LinearAlgebra;
using ZTR.AI.Common.Core.RandomEngines;

namespace ZTR.AI.Algorithms.Core.PSO
{
    class Particle
    {
        private readonly Func<Vector<double>, double> _functionToOptimize;
        private IRandomEngine _randomEngine;
        private readonly Vector<double> _minimumSolutionRange;
        private readonly Vector<double> _maximumSolutionRange;
        private double _jakieśW = 0.8;
        private double _localModifier = 0.1;
        private double _globalModifier = 0.1;

        public Particle(Func<Vector<double>, double> functionToOptimize, Vector<double> startingPosition, Vector<double> velocity, 
            IRandomEngine randomEngine, Vector<double> minimumSolutionRange, Vector<double> maximumSolutionRange)
        {
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

            Velocity = _jakieśW * Velocity 
                       + localRandomModifier * _localModifier * (BestKnownPositionByParticle - Position)
                       + globalRandomModifier * _globalModifier + (bestGlobalKnownPosition - Position);

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