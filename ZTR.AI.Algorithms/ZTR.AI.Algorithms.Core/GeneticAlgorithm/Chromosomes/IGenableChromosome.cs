using System.Collections.Generic;
using Newtonsoft.Json;

namespace ZTR.AI.Algorithms.Core.GeneticAlgorithm.Chromosomes;

public interface IGenableChromosome<T> : IChromosome
{
    [JsonIgnore] public IReadOnlyList<T> Genes { get; }
}