namespace ZTR.AI.SimulatedAnnealing.Core;

public static class RandomEngine
{
    public static IRandomEngine GetDefault()
    {
        return new SystemRandomEngine();
    }
}

public static class RandomEngineExtensions
{
    /// <summary>
    /// Returns value in range from -1.0 to 1.0 using random engine.
    /// </summary>
    /// <param name="random"></param>
    /// <returns></returns>
    public static double NextDoubleFromRandomDistribution(this IRandomEngine random)
    {
        return random.NextDouble() * 2.0 - 1.0;
    }
}