using System.Diagnostics.CodeAnalysis;

namespace ZTR.AI.Common.Core.RandomEngines;

public static class RandomEngineExtensions
{
    /// <summary>
    /// Returns value in range from -1.0 to 1.0 using random engine.
    /// </summary>
    /// <param name="random"></param>
    /// <returns></returns>
    public static double NextDoubleFromRandomDistribution([NotNull] this IRandomEngine random)
    {
        return random.NextDouble() * 2.0 - 1.0;
    }
}