using System;

namespace ZTR.AI.Common.Core.RandomEngines;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA5394:Do not use insecure randomness", Justification = "This class shouldn't be used for security.")]
public class SystemRandomEngine : IRandomEngine
{
    private readonly Random _random;

    public SystemRandomEngine()
    {
        _random = new Random();
    }

    public double NextDouble()
    {
        return _random.NextDouble();
    }
}