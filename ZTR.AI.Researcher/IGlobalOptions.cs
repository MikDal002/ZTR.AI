namespace ZT.AI.Researcher
{
    public interface IGlobalOptions
    {
        public FileInfo Output { get; set; }
        public int Repeat { get; set; }
        public int StepsAtEnd { get; set; }
        public int StepsAtBeginning { get; set; }
        public TestFunction TestFunction { get; set; }
    }
}