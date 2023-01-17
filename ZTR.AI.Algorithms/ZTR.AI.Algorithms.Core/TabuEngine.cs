using System;
using System.Collections.Generic;
using Light.GuardClauses;
using Light.GuardClauses.FrameworkExtensions;
using MathNet.Numerics.LinearAlgebra;
using ZTR.AI.Algorithms.Core;
using ZTR.AI.Algorithms.Core.PositionProviders;
using ZTR.AI.Common.Core.RandomEngines;

namespace ZTR.AI.SimulatedAnnealing.Core
{
    public class TabuEngine
    {
        private readonly Func<Vector<double>, double> _functionToOptimize;
        private  int _restartsLeft;
        private readonly int _explorationAmount;
        private int _explorationLeft;
        private readonly Vector<double> _rangeOfTabu;
        private readonly Vector<double> _minimumSolutionRange;
        private readonly Vector<double> _maximumSolutionRange;
        private readonly long _tabuListMax;
        private readonly List<Vector<double>> _tabuList = new();
        private readonly IPositionProvider _globalPositionProvider;
        private IPositionProvider _localPositionProvider;
        private readonly IRandomEngine? _engine;
        private readonly IRandomEngine _randomEngine;

        public Vector<double> CurrentSolution { get; private set; }

        public double Result { get; private set; }
        public bool IsFinished => _restartsLeft <= 0;

        public TabuEngine(Func<Vector<double>, double> functionToOptimize, Vector<double> minimumSolutionRange,
            Vector<double> maximumSolutionRange,  Vector<double> rangeOfTabu, int restartsAmount = 100, int explorationAmount = 100, long tabuListMax = 10, IRandomEngine? engine = null)
        {
            _functionToOptimize = functionToOptimize;
            _restartsLeft = restartsAmount;
            _explorationAmount = _explorationLeft = explorationAmount;

            _rangeOfTabu = rangeOfTabu
                .MustNotBeNull()
                .MustBeTheSameCountAs(minimumSolutionRange);

            _rangeOfTabu.ForEach(d => Check.MustBeGreaterThanOrEqualTo<double>(d, 0));

            _minimumSolutionRange = minimumSolutionRange
                .MustNotBeNull()
                .MustBeTheSameCountAs(maximumSolutionRange);
            _maximumSolutionRange = maximumSolutionRange
                .MustNotBeNull();

            _randomEngine = engine ?? RandomEngine.Default;


            _tabuListMax = tabuListMax;
            _globalPositionProvider = new RandomPositionProvider(_randomEngine);
            _localPositionProvider = GetNewLocalPositionProvider();
            _engine = engine;

            CurrentSolution = _globalPositionProvider.GetNextPosition(Vector<double>.Build.Dense(minimumSolutionRange.Count, 0.0), _maximumSolutionRange, _minimumSolutionRange); 

            Result = double.PositiveInfinity;
        }

        private bool _getNewGlobalPosition;

        public void NextStep()
        {
            if (IsFinished) return;

            if (_explorationLeft <= 0) _getNewGlobalPosition = true;

            if (_getNewGlobalPosition)
            {
                var isSolutionFound = false;
                --_restartsLeft;
                _explorationLeft = _explorationAmount;
                _getNewGlobalPosition = false;
                while (!isSolutionFound)
                {

                    CurrentSolution =
                        _globalPositionProvider.GetNextPosition(CurrentSolution, _maximumSolutionRange, _minimumSolutionRange);
                    _localPositionProvider = GetNewLocalPositionProvider();

                    if (IsInTabuList(CurrentSolution)) continue;
                    isSolutionFound = true;
                    

                    if (_tabuList.Count >= _tabuListMax)
                    {
                        _tabuList.RemoveAt(_tabuList.Count - 1);
                    }
                    _tabuList.Add(CurrentSolution);
                }

            }

            --_explorationLeft;

            var proposedPosition = _localPositionProvider.GetNextPosition(CurrentSolution, _maximumSolutionRange, _minimumSolutionRange);
            var proposedValue = _functionToOptimize(proposedPosition);

            if (proposedValue < Result)
            {
                Result = proposedValue;
                CurrentSolution = proposedPosition;
            }
        }

        private TemperatureKeepAndDownPositionProvider GetNewLocalPositionProvider()
        {
            return new TemperatureKeepAndDownPositionProvider(1, 0.1, _randomEngine);
        }

        private bool IsInTabuList(Vector<double> solutionToCheck)
        {
            
            var count = solutionToCheck.Count;
            foreach (var tabu in _tabuList)
            {
                for (var i = 0; i < count; i++)
                {
                    var isInTabu = Math.Abs(solutionToCheck[i] - tabu[i]) < _rangeOfTabu[i];
                    if (isInTabu) return true;
                }
            }

            return false;
        }
    }
}