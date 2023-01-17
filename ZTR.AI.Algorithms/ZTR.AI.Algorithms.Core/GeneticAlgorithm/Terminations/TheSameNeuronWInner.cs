using Newtonsoft.Json;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Chromosomes;

namespace ZTR.AI.Algorithms.Core.GeneticAlgorithm.Terminations;

public class TheSameNeuronWinner : ITermination
{
    [JsonIgnore] public long AmountLastNeruonWins { get; private set; } = 0;
    private IChromosome? _lastKnwonWinner = null;

    public long MaxGenerationsCount { get; }

    public TheSameNeuronWinner(long maxGenerationsCount)
    {
        MaxGenerationsCount = maxGenerationsCount;
    }

    /// <inheritdoc />
    public bool HasReached(IGeneticAlgorithm geneticAlgorithm)
    {
        if (_lastKnwonWinner != geneticAlgorithm.BestChromosome)
        {
            AmountLastNeruonWins = 0;
            _lastKnwonWinner = geneticAlgorithm.BestChromosome;
        }

        return MaxGenerationsCount < ++AmountLastNeruonWins;
    }
}