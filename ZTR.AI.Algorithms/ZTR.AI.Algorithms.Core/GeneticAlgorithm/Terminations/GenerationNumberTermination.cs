using Light.GuardClauses;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm;

namespace ZTR.AI.Algorithms.Core.GeneticAlgorithm.Terminations;

public class GenerationNumberTermination : ITermination
{
    public int MaxGenerationsCount { get; }

    public GenerationNumberTermination(int maxGenerationsCount)
    {
        MaxGenerationsCount = maxGenerationsCount;
    }

    /// <inheritdoc />
    public bool HasReached(IGeneticAlgorithm geneticAlgorithm)
    {
        geneticAlgorithm.MustNotBeNull();
        return geneticAlgorithm.GenerationsNumber > MaxGenerationsCount;
    }
}