namespace ZTR.AI.SimulatedAnnealing.Core
{
    public interface ITemperatureBasedPositionProvider : IPositionProvider
    {
        double WorkingTemperature { get; }
    }
}