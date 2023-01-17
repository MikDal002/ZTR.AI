using Light.GuardClauses;
using MathNet.Numerics.LinearAlgebra;
using ZTR.AI.Common.Core.RandomEngines;

namespace ZTR.AI.Algorithms.Core.PositionProviders;

public class RandomPositionProvider : IPositionProvider
{
    private readonly IRandomEngine _randomEngine;

    public RandomPositionProvider(IRandomEngine randomEngine)
    {
        _randomEngine = randomEngine;
    }

    public Vector<double> GetNextPosition(Vector<double> currentSolution, Vector<double> maximumSolutionRange, Vector<double> minimumSolutionRange)
    {
        minimumSolutionRange.MustNotBeNull();
        maximumSolutionRange.MustNotBeNull();
        currentSolution.MustNotBeNull();

        return _randomEngine.NextVectorFromRange(minimumSolutionRange, maximumSolutionRange);
    }
}