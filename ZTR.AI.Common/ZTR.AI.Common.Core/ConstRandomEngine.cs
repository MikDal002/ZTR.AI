namespace ZTR.AI.SimulatedAnnealing.Core;

public class ConstRandomEngine : IRandomEngine
{
    private readonly double _value;

    public ConstRandomEngine(double value)
    {
        _value = value;
    }

    public double NextDouble()
    {
        return _value;
    }
}