using System.Collections.Generic;

namespace ZTR.AI.Algorithms.Core.GeneticAlgorithm.Chromosomes;

public interface IGenableChromosomeFactory<T, E> : IChromosomeFactory<T> where T : IGenableChromosome<E>
{
    T FromGenes(IList<E> genes);
    E GetGene(int geneNumber);
}

public interface IChromosomeFactory<T> where T : IChromosome
{
    T CreateNew();
}