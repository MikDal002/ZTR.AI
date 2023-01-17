using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Chromosomes;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Extensions;

namespace ZTR.AI.Algorithms.Core.GeneticAlgorithm.Solutions.TravelersSalesmanProblem;

public class TravelerProblemFactory : IGenableChromosomeFactory<TravelerProblemChromosome, City>
{
    public long Counter { get; private set; }

    private string FilePath { get; } = "SalesManData/ATT48.txt"; // Shortest path is 33523

    //private string FilePath { get; } = "SalesManData/P01.txt"; // Shortest path is 291
    //private string FilePath { get; } = "SalesManData/ATT48.txt";
    private List<City> CitiesFromFile { get; } = new List<City>();
    public IReadOnlyList<City> AllCities => CitiesFromFile;

    private void LoadCitiesFromFile()
    {
        var lines = File.ReadAllLines(FilePath);
        var i = 0;
        foreach (var cityRaw in lines.Where(d => !string.IsNullOrWhiteSpace(d)))
        {
            if (cityRaw.StartsWith("#")) continue;
            var split = cityRaw.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (split.Length != 2) throw new ArgumentException("There is abnormal amount of points for the city");
            var city = new City
            {
                Location = new PointF(float.Parse(split[0]), float.Parse(split[1])),
                Name = $"City {++i}"
            };
            CitiesFromFile.Add(city);
        }
    }

    /// <inheritdoc />
    TravelerProblemChromosome IChromosomeFactory<TravelerProblemChromosome>.CreateNew()
    {
        if (CitiesFromFile.Count == 0) LoadCitiesFromFile();
        ++Counter;
        return new TravelerProblemChromosome(CitiesFromFile.Shuffle());
    }

    /// <inheritdoc />
    public TravelerProblemChromosome FromGenes(IList<City> genes)
    {
        ++Counter;
        return new TravelerProblemChromosome(genes);
    }

    /// <inheritdoc />
    public City GetGene(int geneNumber)
    {
        throw new NotImplementedException();
    }
}