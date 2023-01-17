using ZT.AI.Researcher;

namespace ZTR.AI.Researcher.SimulatedAnnealing;

public record SimulatedAnnealingOptions : IGlobalOptions
{

    #region IGlobalOptions
    public FileInfo Output { get; set; }
    public int Repeat { get; set; }
    public int StepsAtEnd { get; set; }
    public int StepsAtBeginning { get; set; }
    public TestFunction TestFunction { get; set; }

    #endregion

    public double StartingTemperature { get; set; } = 100;
}