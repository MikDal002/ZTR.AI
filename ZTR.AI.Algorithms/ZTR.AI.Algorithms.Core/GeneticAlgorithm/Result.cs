using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Chromosomes;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Crossovers;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Mutations;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Selections;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Terminations;
// ReSharper disable UnusedMember.Global
#pragma warning disable CS8618

namespace ZTR.AI.Algorithms.Core.GeneticAlgorithm;

public class StepDef
{
    public int generation { get; set; }
    public long Elapse { get; set; }
    public double fitness { get; set; }
}

public class Result<TChrom> where TChrom : class, IChromosome
{
    public string SelectionName => Selection.GetType().Name;
    public ISelection Selection { get; set; }
    public string CrossoverName => Crossover.GetType().Name;
    public ICrossover<TChrom> Crossover { get; set; }
    public string MutationName => Mutation.GetType().Name;
    public IMutation<TChrom> Mutation { get; set; }
    public string TerminationName => Termination.GetType().Name;
    public ITermination Termination { get; set; }
    public int Population { get; set; }


    public TChrom WinnerChromosome { get; set; }
    public int AmountOfGenerations { get; set; }

    public double RealTheBestValue { get; set; }
    public double TheBestFoundFitness { get; set; }
    public long TotalTimeMs { get; set; }


    public List<StepDef> Steps { get; /*set;*/ } = new();

    public bool IsEqualToResult(JObject otherJson)
    {
        var thisJs = JObject.FromObject(this);
        return AreJObjectResultsEqual(otherJson, thisJs);
    }

    public static bool AreJObjectResultsEqual(JObject otherJson, JObject thisJs)
    {
        var toCheck = new[]
        {
            nameof(SelectionName), nameof(CrossoverName), nameof(Crossover),
            nameof(MutationName), nameof(Mutation), nameof(TerminationName), nameof(Termination),
            nameof(Population)
        };

        foreach (var chk in toCheck)
        {
            var that = thisJs[chk];
            var another = otherJson[chk];

            if (!JToken.DeepEquals(that, another))
                //if (!string.Equals(that.ToString(), another.ToString(), StringComparison.InvariantCultureIgnoreCase))
                return false;
        }

        return true;
    }

    public bool IsEqualToResult(Result<TChrom> chrom)
    {
        var otherJson = JObject.FromObject(chrom);
        return IsEqualToResult(otherJson);
    }
}

public class MyComparer<TChrom> : IEqualityComparer<JObject> where TChrom : class, IChromosome
{
    public bool Equals(JObject? x, JObject? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;

        return Result<TChrom>.AreJObjectResultsEqual(x, y);
    }

    public int GetHashCode(JObject obj)
    {
        return (int)obj.Type;
    }
}