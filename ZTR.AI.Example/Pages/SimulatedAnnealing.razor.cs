using System.Diagnostics;
using ZTR.AI.Example.Shared;
using ZTR.AI.SimulatedAnnealing.Core;

namespace ZTR.AI.Example.Pages
{
    public class SimulatedAnnealingExample
    {
        public static SimulatedAnnealingExample Sin { get; } = new()
        {
            Name = "Sin",
            Function = Math.Sin,
            Step = 0.1,
            Min = -Math.PI * 1.1,
            Max = Math.PI * 1.1
        };

        public static SimulatedAnnealingExample Cos { get; } = new()
        {
            Name = "Cos",
            Function = Math.Cos,
            Step = 0.1,
            Min = -Math.PI * 1.1,
            Max = Math.PI * 1.1
        };

        public static SimulatedAnnealingExample ShortExponental { get; } = new()
        {
            Name = "Sinus&Exponental",
            Function = x => -(x + Math.Sin(x)) * Math.Exp(-Math.Pow(x, 2.0)),
            Step = 0.1,
            Min = -Math.PI * 1.1,
            Max = Math.PI * 1.1
        };


        public string Name { get; init; }
        public IEnumerable<DataItem> DrawingData =>
            MyMoreLinq.Range(Min, Max, Step).Select(d => new DataItem(d, Function(d)));
        public Func<double, double> Function { get; init; }
        public double Step { get; init; } = 0.1;
        public double Min { get; init; } = -Math.PI;
        public double Max { get; init; } = Math.PI;
    }

    public partial class SimulatedAnnealing
    {
        public IEnumerable<SimulatedAnnealingExample> Examples { get; } = new[]
        {
            SimulatedAnnealingExample.Sin, SimulatedAnnealingExample.Cos,  SimulatedAnnealingExample.ShortExponental, 
        };
        public SimulatedAnnealingExample CurrentExample { get; private set; } = SimulatedAnnealingExample.Cos;

        public double CurrentSolution { get; private set; }
        public double CurrentResult { get; private set; }
        public double CurrentIteration { get; private set; }
        public bool IsRunning { get; private set; }
        public List<(int Step, double X, double Value)> History { get; } = new();

        void OnChange(object value)
        {
            //var example = value as SimulatedAnnealingExample;
            //Debug.Assert(example != null);

            //CurrentExample = example;
            //StateHasChanged();
        }

        private void StartSimulatedAnnealing()
        {
            IsRunning = true;
            var simualatedAnnealingEngine = new SimualatedAnnealingEngine(CurrentExample.Function, 100, 0.0001,
                minimumSolutionRange: CurrentExample.Min, maximumSolutionRange: CurrentExample.Max);
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
                        }
                        await Task.Delay(1);
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
