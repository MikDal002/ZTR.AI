using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Chromosomes;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Extensions;

namespace ZTR.AI.Algorithms.Core.GeneticAlgorithm.Solutions.KnapsackProblem;

public class KnapsackProblemFactory : IGenableChromosomeFactory<KnapsackProblemChromosome, Insert>
{
    public long Counter { get; private set; }

    // public double MaxWeight { get; set; } = 49877f;
    // private string FilePath { get; } = "KnapsackData/knapPI_1_10000_1000_1.txt";

    public double MaxWeight { get; set; } = 1008f;
    private string FilePath { get; } = "KnapsackData/knapPI_2_200_1000_1.txt";

    private List<Insert> InsertsFromFile { get; } = new List<Insert>();
    public IReadOnlyList<Insert> AllInserts => InsertsFromFile;

    private void LoadCitiesFromFile()
    {
        var lines = File.ReadAllLines(FilePath);
        var i = 0;
        foreach (var cityRaw in lines.Where(d => !string.IsNullOrWhiteSpace(d)))
        {
            if (cityRaw.StartsWith("#")) continue;
            var split = cityRaw.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (split.Length != 2) throw new ArgumentException("There is abnormal amount of points for the city");
            var city = new Insert
            {
                Profit = double.Parse(split[0]),
                Weight = double.Parse(split[1]),
                Name = $"Insert {++i}"
            };
            InsertsFromFile.Add(city);
        }
    }

    /// <inheritdoc />
    KnapsackProblemChromosome IChromosomeFactory<KnapsackProblemChromosome>.CreateNew()
    {
        if (InsertsFromFile.Count == 0) LoadCitiesFromFile();
        ++Counter;
        return new KnapsackProblemChromosome(MaxWeight, InsertsFromFile.Shuffle());
    }

    /// <inheritdoc />
    public KnapsackProblemChromosome FromGenes(IList<Insert> genes)
    {
        ++Counter;
        return new KnapsackProblemChromosome(MaxWeight, genes);
    }

    /// <inheritdoc />
    public Insert GetGene(int geneNumber)
    {
        throw new NotImplementedException();
    }
}