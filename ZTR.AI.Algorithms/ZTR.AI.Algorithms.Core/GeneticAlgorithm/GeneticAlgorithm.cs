using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Chromosomes;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Fitnesses;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Populations;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Terminations;
#pragma warning disable CS8618

namespace ZTR.AI.Algorithms.Core.GeneticAlgorithm;

public class GeneticAlgorithm<T> : IGeneticAlgorithm where T : class, IChromosome
{
    public event EventHandler<Generation<T>> GenerationHasGone;

    public IPopulation<T> Population { get; }

    public IFitness<T> Fitness { get; }

    public ITermination Termination { get; set; }

    public T BestChromosome { get; private set; }
    IChromosome IGeneticAlgorithm.BestChromosome => BestChromosome;
    public int GenerationsNumber { get; private set; }

    public GeneticAlgorithm(IPopulation<T> population, IFitness<T> fitness)
    {
        Population = population ?? throw new ArgumentNullException(nameof(population));
        Fitness = fitness ?? throw new ArgumentNullException(nameof(fitness));
    }


    public IEnumerable<double> Start()
    {
        do
        {
            GenerationsNumber++;
            var currentGeneration = Population.StartNewGeneration();
            if (currentGeneration.Count == 0)
                throw new ArgumentException("Generation must be bigger than zero!");


            foreach(var chromosome in currentGeneration) 
            {
                if (chromosome.Fitness != null) continue;
                chromosome.Fitness = Fitness.Evaluate(chromosome);
                yield return chromosome.Fitness.Value;
                if (chromosome.Fitness == null) throw new ArgumentException("Fitness must be bigger than zero!");
            }
            currentGeneration.BestChromosome = currentGeneration.Max(d => d);

            if (BestChromosome == null) BestChromosome = currentGeneration.BestChromosome;
            else if (currentGeneration.BestChromosome.CompareTo(BestChromosome) > 0)
                BestChromosome = currentGeneration.BestChromosome;

            GenerationHasGone?.Invoke(this, currentGeneration);
        } while (!Termination.HasReached(this));
    }
}