namespace ZT.AI.Researcher
{
    public record GreedyOptions : IGlobalOptions
    {
        public int StepsAtEnd { get; set; }
        public int StepsAtBeginning { get; set; }
        public TestFunction TestFunction { get; set; }
        public FileInfo Output { get; set; }
        public int Repeat { get; set; }
    }
}