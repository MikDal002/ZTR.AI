using System;
using System.Collections.Generic;
using System.Linq;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Chromosomes;

namespace ZTR.AI.Algorithms.Core.GeneticAlgorithm.Solutions.MyProblem;

public record MyProblemChromosome : FitnessComparableChromosome, IGenableChromosome<double>
{
    private readonly List<double> _genes = new List<double>();

    public double X1
    {
        get => _genes[0];
        set => _genes[0] = value;
    }

    public double X2
    {
        get => _genes[1];
        set => _genes[1] = value;
    }

    public double Y1
    {
        get => _genes[2];
        set => _genes[2] = value;
    }

    public double Y2
    {
        get => _genes[3];
        set => _genes[3] = value;
    }

    public MyProblemChromosome(IList<double> genes)
    {
        if (genes.Count != 4) throw new ArgumentException();
        //if (genes.Any(d => d > max || d < min)) throw new ArgumentException();
        _genes = genes as List<double> ?? genes.ToList();
    }

    public MyProblemChromosome(double x1, double x2, double y1, double y2) : this(new List<double>
    {
        x1,
        x2,
        y1,
        y2
    })
    { }

    /// <inheritdoc />
    public IChromosome FromGenes(IList<double> genes)
    {
        return new MyProblemChromosome(genes);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{X1}x{X2};{Y1}x{Y2}";
    }

    /// <inheritdoc />
    public IReadOnlyList<double> Genes => _genes;
}