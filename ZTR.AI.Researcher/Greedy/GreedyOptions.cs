﻿using ZT.AI.Researcher;

namespace ZTR.AI.Researcher.Greedy;

public record GreedyOptions : IGlobalOptions
{

    #region IGlobalOptions
    public FileInfo Output { get; set; }
    public int Repeat { get; set; }
    public int StepsAtEnd { get; set; }
    public int StepsAtBeginning { get; set; }
    public TestFunction TestFunction { get; set; }
    public string AlgorithmName { get; set; }

    #endregion
}