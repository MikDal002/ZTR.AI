using System;

namespace ZTR.AI.SimulatedAnnealing.Core;

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