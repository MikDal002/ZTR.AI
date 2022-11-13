using MathNet.Numerics.LinearAlgebra;

namespace ZTR.AI.Example.Pages
{
    public class ExampleHistory
    {
        private readonly List<(int Step, Vector<double> X, double Value)> _iterationResults = new();
        public Vector<double> CurrentSolution { get; private set; }
        public double CurrentResult { get; private set; }
        public double CurrentIteration { get; private set; }
        public bool IsRunning { get; private set; }

        public IReadOnlyCollection<(int Step, Vector<double> X, double Value)> IterationResults => _iterationResults;

        public ExampleHistory()
        {
            CurrentSolution = Vector<double>.Build.Dense(1);
        }

        public void Start()
        {
            IsRunning = true;
        }

        public void Stop()
        {
            IsRunning = false;
        }
    

        public void Update(double result, Vector<double> solution, int iterationNo)
        {
            CurrentResult = result;
            CurrentSolution = solution;
            _iterationResults.Add((iterationNo, CurrentSolution, CurrentResult));
            CurrentIteration = iterationNo;
        }
    }
}