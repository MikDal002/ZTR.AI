using MathNet.Numerics.LinearAlgebra;

namespace ZTR.AI.Algorithms.Core.PositionProviders
{
    public interface IPositionProvider
    {
        Vector<double> GetNextPosition(Vector<double> currentSolution, Vector<double> maximumSolutionRange, Vector<double> minimumSolutionRange);
    }
}