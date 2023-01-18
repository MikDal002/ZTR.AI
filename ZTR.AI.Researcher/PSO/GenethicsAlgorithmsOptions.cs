using System.CommandLine.Builder;
using ZT.AI.Researcher;

namespace ZTR.AI.Researcher.PSO;


public record ParticleSwarmOptimisationAlgorithmsOptions : IGlobalOptions
{
    #region IGlobalOptions
    public FileInfo Output { get; set; }
    public int Repeat { get; set; }
    public int StepsAtEnd { get; set; }
    public int StepsAtBeginning { get; set; }
    public TestFunction TestFunction { get; set; }

    #endregion

}