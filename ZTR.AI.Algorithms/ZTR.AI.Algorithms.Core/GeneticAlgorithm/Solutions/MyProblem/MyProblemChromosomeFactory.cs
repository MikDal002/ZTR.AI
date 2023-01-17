using System;
using System.Collections.Generic;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Chromosomes;

namespace ZTR.AI.Algorithms.Core.GeneticAlgorithm.Solutions.MyProblem;

public class MyProblemChromosomeFactory : IGenableChromosomeFactory<MyProblemChromosome, double>
{
    private static readonly Random random = new Random();
    private readonly int _max = 10000;
    private readonly int _min = 1;


    public MyProblemChromosomeFactory(int max, int min)
    {
        _max = max;
        _min = min;
    }



    /// <inheritdoc />
    public MyProblemChromosome CreateNew()
    {
        return new MyProblemChromosome(GetGene(0), GetGene(1), GetGene(2), GetGene(3));
    }

    public MyProblemChromosome FromGenes(IList<double> genes)
    {
        return new MyProblemChromosome(genes);
    }

    /// <inheritdoc />
    public double GetGene(int geneNumber)
    {
        return random.Next(_min, _max);
    }
}