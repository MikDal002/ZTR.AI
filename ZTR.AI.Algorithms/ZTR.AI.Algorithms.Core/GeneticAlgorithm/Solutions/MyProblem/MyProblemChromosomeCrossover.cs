using System;
using System.Collections.Generic;
using ZTR.AI.Algorithms.Core.GeneticAlgorithm.Crossovers;

namespace ZTR.AI.Algorithms.Core.GeneticAlgorithm.Solutions.MyProblem;

public class MyProblemChromosomeCrossover : ICrossover<MyProblemChromosome>
{
    public MyProblemChromosomeCrossover(int requiredNumberOfParents = 2)
    {
        RequiredNumberOfParents = requiredNumberOfParents;
    }

    /// <inheritdoc />
    public int RequiredNumberOfParents { get; }

    /// <inheritdoc />
    public IEnumerable<MyProblemChromosome> MakeChildren(IEnumerable<MyProblemChromosome> parents)
    {
        if (RequiredNumberOfParents != 2) throw new NotImplementedException();
        MyProblemChromosome? previousParent = null;
        foreach (var parent in parents)
        {
            if (previousParent == null)
            {
                previousParent = parent;
                continue;
            }

            yield return new MyProblemChromosome(previousParent.X1, parent.X2, previousParent.Y1, parent.Y2);
            yield return new MyProblemChromosome(parent.X1, previousParent.X2, parent.Y1, previousParent.Y2);
        }
    }
}