using System.Collections;
using System.Collections.Generic;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Chromosomes;

namespace ZTR.AI.Algorithms.Core.GeneticAlgorithm;

public class Generation<T> : IReadOnlyList<T> where T : IChromosome
{
    private readonly IReadOnlyList<T> _readOnlyCollectionImplementation;

    public T? BestChromosome { get; set; }

    public Generation(IReadOnlyList<T> list)
    {
        _readOnlyCollectionImplementation = list;
    }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator()
    {
        return _readOnlyCollectionImplementation.GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_readOnlyCollectionImplementation).GetEnumerator();
    }

    /// <inheritdoc />
    public int Count => _readOnlyCollectionImplementation.Count;

    /// <inheritdoc />
    public T this[int index] => _readOnlyCollectionImplementation[index];
}