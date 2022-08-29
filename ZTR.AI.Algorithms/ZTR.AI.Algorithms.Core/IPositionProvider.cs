namespace ZTR.AI.SimulatedAnnealing.Core
{
    public interface IPositionProvider
    {
        double GetNextPosition(double currentSolution, double maximumSolutionRange, double minimumSolutionRange);
        bool IsFinished { get; }
    }
}