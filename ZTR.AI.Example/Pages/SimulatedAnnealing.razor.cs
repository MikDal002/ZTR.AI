using ZTR.AI.Example.Shared;
using ZTR.AI.SimulatedAnnealing.Core;

namespace ZTR.AI.Example.Pages
{
    public partial class SimulatedAnnealing
    {
        public Func<double, double> SinFunction => Math.Sin;
        public double Step { get; } = 0.1;
        public double Min { get; } = -Math.PI;
        public double Max { get; } = Math.PI;
        public IEnumerable<DataItem> SinData => MyMoreLinq.Range(Min, Max, Step).Select(d => new DataItem(d, SinFunction(d)));
        public double CurrentSolution { get; private set; }
        public double CurrentResult { get; private  set; }
        public double CurrentIteration { get; private set; }
        public bool IsRunning { get; private set; }
        public List<(int Step, double X, double Value)> History { get; } = new();
        private void StartSimulatedAnnealing()
        {
            IsRunning = true;
            var simualatedAnnealingEngine = new SimualatedAnnealingEngine(SinFunction, 100, 0.0001,
                minimumSolutionRange: Min, maximumSolutionRange:Max);
            int i = 0;
            History.Clear();

            Task.Run(async () =>
            {
                double prevTemperature = 0.0;
                   while (!simualatedAnnealingEngine.IsFinished)
                   {
                       simualatedAnnealingEngine.NextStep();
                       if (i % 100 == 0)
                       {
                           if (Math.Abs(prevTemperature - simualatedAnnealingEngine.WorkingTemperature) > 0.000001)
                           {
                               prevTemperature = simualatedAnnealingEngine.WorkingTemperature;
                               CurrentResult = simualatedAnnealingEngine.Result;
                               CurrentSolution = simualatedAnnealingEngine.CurrentSolution;
                               History.Add((i, CurrentSolution, CurrentResult));
                               CurrentIteration = i;
                               StateHasChanged();
                               await Task.Delay(1);
                        }
                            
                       }
                       i++;
                   }
                   CurrentResult = simualatedAnnealingEngine.Result;
                   CurrentSolution = simualatedAnnealingEngine.CurrentSolution;
                   CurrentIteration = i;
                   History.Add((i, CurrentSolution, CurrentResult));
                   IsRunning = false;
                   StateHasChanged();
               });
        }
    }
}
