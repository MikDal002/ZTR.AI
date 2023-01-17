using ZTR.AI.Algorithms.Core.GeneticAlgorithm;

namespace ZTR.AI.Algorithms.Core.GeneticAlgorithm.Terminations;

public interface ITermination
{
    bool HasReached(IGeneticAlgorithm geneticAlgorithm);
}