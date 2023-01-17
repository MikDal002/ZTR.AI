using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Light.GuardClauses;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Chromosomes;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Crossovers;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Mutations;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Selections;

namespace ZTR.AI.Algorithms.Core.GeneticAlgorithm.Populations;

public class Population<T> : IPopulation<T> where T : IChromosome
{
    private readonly IChromosomeFactory<T> _adamFactory;
    private readonly ICrossover<T> _crossover;
    private readonly IMutation<T> _mutation;
    private readonly ISelection _selection;
    private Generation<T>? _previousGeneration;

    public int MinSize { get; }
    public int MaxSize { get; }

    public Population(int minSize, int maxSize, IChromosomeFactory<T> adamFactory,
        ICrossover<T> crossover, IMutation<T> mutation, ISelection selection)
    {
        _adamFactory = adamFactory ?? throw new ArgumentNullException(nameof(adamFactory));
        _crossover = crossover ?? throw new ArgumentNullException(nameof(crossover));
        _mutation = mutation;
        _selection = selection ?? throw new ArgumentNullException(nameof(selection));
        MinSize = minSize;
        MaxSize = maxSize;
    }

    /// <inheritdoc />
    public Generation<T> StartNewGeneration()
    {
        var chromosomesForPopulation = new List<T>(MinSize);
        if (_previousGeneration == null)
        {
            for (var i = 0; i < MinSize; ++i) chromosomesForPopulation.Add(_adamFactory.CreateNew());
        }
        else
        {
            var locker = new object();
            // TODO MD 24-04-2021:  This shouldn't be hardcoded!
            chromosomesForPopulation.Add(_previousGeneration.BestChromosome);
            if (_previousGeneration.Count == 1) Debug.WriteLine("I'm only one!");
            Parallel.For(chromosomesForPopulation.Count, MinSize, () => new List<T>(),
                (i, state, sublist) =>
                {
                    try
                    {
                        if (chromosomesForPopulation.Count > MaxSize)
                        {
                            Debug.WriteLine("Za dużo dzieciaków, więc nie produkuję nowych.");
                            state.Stop();
                        }

                        var kids = GetChildren(_previousGeneration).ToList();
                        //if (kids.Count == 0) Debug.WriteLine("Czemu nie masz dzieci?");
                        sublist.AddRange(kids);

                        return sublist;
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                        throw;
                    }
                },
                newKids =>
                {
                    lock (locker)
                    {
                        //if (newKids.Count == 0) Debug.WriteLine("Ups!");
                        foreach (var kid in newKids /*.Where(d => d != null)*/)
                        {
                            if (chromosomesForPopulation.Count <= MaxSize)
                                chromosomesForPopulation.Add(kid);
                            else
                                Debug.WriteLine("Za dużo dzieciaków, więc nie dodaję.");
                        }
                    }
                }
            );
        }

        //             for (int i = MinSize - chromosomesForPopulation.Count; i > 0; --i)
        //                 chromosomesForPopulation.Add(_adamFactory.CreateNew());

        //Debug.Assert(chromosomesForPopulation.Count >= MinSize);
        return _previousGeneration = new Generation<T>(chromosomesForPopulation);
    }

    private IEnumerable<T> GetChildren(Generation<T> previousGeneration)
    {
        var parents = _selection.SelectChromosomes(previousGeneration, _crossover.RequiredNumberOfParents)
            .ToList();
        if (parents.Count != _crossover.RequiredNumberOfParents)
        {
            //Debug.WriteLine($"Amount of parents isn't sufficient ({parents.Count} vs {_crossover.RequiredNumberOfParents})!");
            yield break;
        }

        IEnumerable<T> offsprings;
#pragma warning disable CA1031 // Nie przechwytuj ogólnych typów wyjątków
        try { offsprings = _crossover.MakeChildren(parents); }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            yield break;
            //throw;
        }
#pragma warning restore CA1031 // Nie przechwytuj ogólnych typów wyjątków

        foreach (var offspring in offsprings)
        {
            var mutatedSprings = _mutation.Mutate(offspring);
            yield return mutatedSprings ?? offspring;
        }
    }
}