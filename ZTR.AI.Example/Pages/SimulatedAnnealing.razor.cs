using ZTR.AI.Example.Shared;
using ZTR.AI.SimulatedAnnealing.Core;

namespace ZTR.AI.Example.Pages
{
    public partial class SimulatedAnnealing
    {
        public Func<double, double> SinFunction => Math.Sin;
        public double Step { get; } = 0.1;
        public double Min { get; } = -Math.Tau;
        public double Max { get; } = Math.Tau;
        public IEnumerable<DataItem> SinData => MyMoreLinq.Range(Min, Max, Step).Select(d => new DataItem(d, SinFunction(d)));
        public double CurrentSolution { get; set; }
        public double CurrentResult { get; set; }
        public List<(int Step, double X, double Value)> History { get; } = new();
        private void StartSimulatedAnnealing()
        {
            var simualatedAnnealingEngine = new SimualatedAnnealingEngine(SinFunction, 100, 0.0001,
                minimumSolutionRange: Min, maximumSolutionRange:Max);
            int i = 0;
            History.Clear();

            Task.Run(async () =>
               {
                   while (!simualatedAnnealingEngine.IsFinished)
                   {
                       simualatedAnnealingEngine.NextStep();
                       if (i % 10 == 0)
                       {
                           CurrentResult = simualatedAnnealingEngine.Result;
                           CurrentSolution = simualatedAnnealingEngine.CurrentSolution;
                           History.Add((i, CurrentSolution, CurrentResult));
                           StateHasChanged();
                           await Task.Delay(1);
                       }
                       i++;
                   }
                   CurrentResult = simualatedAnnealingEngine.Result;
                   CurrentSolution = simualatedAnnealingEngine.CurrentSolution;
                   History.Add((i, CurrentSolution, CurrentResult));
                   StateHasChanged();
               });
        }
    }
}
