using System.CommandLine.Builder;
using ZT.AI.Researcher;

namespace ZTR.AI.Researcher.GA;


public record GenethicsAlgorithmsOptions : IGlobalOptions
{
    #region IGlobalOptions
    public FileInfo Output { get; set; }
    public int Repeat { get; set; }
    public int StepsAtEnd { get; set; }
    public int StepsAtBeginning { get; set; }
    public TestFunction TestFunction { get; set; }

    #endregion


    public double UniformCrossOverRation { get; set; } = 0.1;
}