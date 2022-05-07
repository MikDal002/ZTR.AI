namespace ZTR.AI.SimulatedAnnealing.Core;

public interface IRandomEngine
{
    /// <summary>
    /// Return value from 0.0 to 1.0
    /// </summary>
    /// <returns></returns>
    double NextDouble();
}