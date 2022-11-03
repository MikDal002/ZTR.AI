namespace ZTR.AI.Algorithms.Core.PositionProviders
{
    public interface IPositionProvider
    {
        double GetNextPosition(double currentSolution, double maximumSolutionRange, double minimumSolutionRange);
    }
}