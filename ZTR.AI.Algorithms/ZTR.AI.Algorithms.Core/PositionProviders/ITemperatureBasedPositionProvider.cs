namespace ZTR.AI.Algorithms.Core.PositionProviders
{
    public interface ITemperatureBasedPositionProvider : IPositionProvider
    {
        double WorkingTemperature { get; }
        bool IsFinished { get; }
    }
}