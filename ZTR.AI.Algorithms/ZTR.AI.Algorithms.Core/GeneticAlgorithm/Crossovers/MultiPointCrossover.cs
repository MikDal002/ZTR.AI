using System;
using System.Collections.Generic;
using System.Linq;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Chromosomes;

namespace ZTR.AI.Algorithms.Core.GeneticAlgorithm.Crossovers;

public class MultiPointCrossover<T, E> : ICrossover<T> where T : IGenableChromosome<E>
{
    private readonly int _amountOfPoints;
    private readonly IGenableChromosomeFactory<T, E> _factory;
    private List<int>? _splitPoints;

    public MultiPointCrossover(int amountOfRandomPoints, IGenableChromosomeFactory<T, E> factory)
    {
        _amountOfPoints = amountOfRandomPoints;
        _factory = factory;
    }

    public MultiPointCrossover(IEnumerable<int> corssOversPoints, IGenableChromosomeFactory<T, E> factory)
    {
        _factory = factory;
        _splitPoints = corssOversPoints.ToList();
        _splitPoints.Sort();
        _amountOfPoints = _splitPoints.Count;
    }

    /// <inheritdoc />
    public int RequiredNumberOfParents => 2;

    /// <inheritdoc />
    public IEnumerable<T> MakeChildren(IEnumerable<T> parents)
    {
        var list = parents.ToList();
        if (list.Count != RequiredNumberOfParents)
            throw new ArgumentException("The number of parents isn't sufficient", nameof(parents));
        if (list[0].Genes.Count != list[1].Genes.Count)
            throw new ArgumentException("Different size of genes is not supported here!");

        var maxCount = list[0].Genes.Count;
        var childGenes1 = new List<E>();
        var childGenes2 = new List<E>();
        var random = new Random();

        if (_splitPoints == null || _splitPoints.Count == 0)
        {
            _splitPoints = new List<int>(_amountOfPoints);
            for (var j = 0; j < _amountOfPoints; ++j) _splitPoints.Add(random.Next(maxCount - 1));
            _splitPoints.Sort();
        }

        for (var i = 0; i < maxCount; ++i)
        {
            var next = _splitPoints.Count(d => d >= i) % 2;
            childGenes1.Add(next == 0 ? list[0].Genes[i] : list[1].Genes[i]);
            childGenes2.Add(next == 1 ? list[0].Genes[i] : list[1].Genes[i]);
        }

        yield return _factory.FromGenes(childGenes1);
        yield return _factory.FromGenes(childGenes2);
    }
}