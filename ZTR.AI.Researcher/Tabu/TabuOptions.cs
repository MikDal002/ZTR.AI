using System.CommandLine.Builder;
using ZT.AI.Researcher;

namespace ZTR.AI.Researcher.Tabu;

public record TabuOptions : IGlobalOptions
{
    #region IGlobalOptions
    public FileInfo Output { get; set; }
    public int Repeat { get; set; }
    public int StepsAtEnd { get; set; }
    public int StepsAtBeginning { get; set; }
    public TestFunction TestFunction { get; set; }
    public string AlgorithmName { get; set; }

    #endregion

    public double TabuRange { get; set; } = 0.5;
    public int RestartsAmount { get; set; } = int.MaxValue;
    public int ExplorationAmount { get; set; } = 100;
    public int TabuListMax { get; set; } = int.MaxValue;
}