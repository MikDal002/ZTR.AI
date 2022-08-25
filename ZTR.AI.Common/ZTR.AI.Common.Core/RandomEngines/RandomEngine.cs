namespace ZTR.AI.Common.Core.RandomEngines;

public static class RandomEngine
{
    public static IRandomEngine Default { get; } = new SystemRandomEngine();
}