using System;
using System.Collections.Generic;
using System.Linq;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Chromosomes;

namespace ZTR.AI.Algorithms.Core.GeneticAlgorithm.Crossovers;

public class CyclicOrderedCrossover<T, E> : ICrossover<T> where T : IGenableChromosome<E>
{
    private static readonly Random _random = new();
    public int? Length { get; }
    private readonly IGenableChromosomeFactory<T, E> _factory;

    public CyclicOrderedCrossover(IGenableChromosomeFactory<T, E> chromosomeFactory) : this(null,
        chromosomeFactory)
    { }

    public CyclicOrderedCrossover(int? length, IGenableChromosomeFactory<T, E> factory)
    {
        Length = length;
        _factory = factory;
    }

    /// <inheritdoc />
    public int RequiredNumberOfParents { get; } = 2;

    /// <inheritdoc />
    public IEnumerable<T> MakeChildren(IEnumerable<T> parentsRaw)
    {
        var parents = parentsRaw.ToList();
        if (parents.Count != RequiredNumberOfParents)
            throw new ArgumentException("The number of parents isn't sufficient", nameof(parentsRaw));
        if (parents[0].Genes.Count != parents[1].Genes.Count)
            throw new ArgumentException("Different size of genes is not supported here!");

        var maxCount = parents[0].Genes.Count;
        var cycleLength = Length ?? _random.Next(maxCount);

        if (cycleLength >= maxCount)
            throw new InvalidOperationException("Length of cycle is lower than length of chromosome!");


        var i = 0;


        {
            i = 0;
            var child1Prime = parents[0].Genes.GroupBy(_ => i++ / cycleLength).Where(d => d.Key % 2 == 0).ToList();
            i = 0;
            var child1Supplement = parents[1].Genes.Where(d => !child1Prime.Any(e => e.Contains(d)))
                .GroupBy(_ => i++ / cycleLength).ToList();
            var readyKid1 = new List<E>();
            i = 0;
            foreach (var kidPart in child1Prime)
            {
                readyKid1.AddRange(kidPart);
                if (i >= child1Supplement.Count) break;
                readyKid1.AddRange(child1Supplement.ElementAt(i++));
            }

            yield return _factory.FromGenes(readyKid1);
        }
        {
            i = 0;
            var child2Prime = parents[1].Genes.GroupBy(_ => i++ / cycleLength).Where(d => d.Key % 2 == 0).ToList();


            i = 0;
            var child2Supplement = parents[1].Genes.Where(d => !child2Prime.Any(e => e.Contains(d)))
                .GroupBy(_ => i++ / cycleLength).ToList();
            var readyKid2 = new List<E>();
            i = 0;
            foreach (var kidPart in child2Prime)
            {
                readyKid2.AddRange(kidPart);
                if (i >= child2Supplement.Count) break;
                readyKid2.AddRange(child2Supplement.ElementAt(i++));
            }

            yield return _factory.FromGenes(readyKid2);
        }
    }
}