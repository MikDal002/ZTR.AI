using System.Diagnostics;
using ZTR.AI.SimulatedAnnealing.Core;

namespace ZTR.AI.Example.Pages;

public partial class SimulatedAnnealingPage
{
    public IEnumerable<SingleDimensionalExample> Examples { get; } = new[]
    {
        SingleDimensionalExample.Sin, SingleDimensionalExample.Cos,  SingleDimensionalExample.ShortExponental, 
    };
    public SingleDimensionalExample CurrentExample { get; private set; } = SingleDimensionalExample.Cos;

    public double CurrentSolution { get; private set; }
    public double CurrentResult { get; private set; }
    public double CurrentIteration { get; private set; }
    public double EndingTemperature { get; set; } = 0.1;
    public bool IsRunning { get; private set; }
    public int StartingTemperature { get; set; } = 100;
    private List<(int Step, double X, double Value)> History { get; } = new();

    public async Task StartSimulatedAnnealing()
    {
        IsRunning = true;
        var simulatedAnnealingEngine = new SimulatedAnnealingEngine(CurrentExample.Function, StartingTemperature, EndingTemperature,
            minimumSolutionRange: CurrentExample.Min, maximumSolutionRange: CurrentExample.Max);
        History.Clear();

        await PerformAlgorithmAsync(simulatedAnnealingEngine).ConfigureAwait(false);
    }

    private async Task PerformAlgorithmAsync(SimulatedAnnealingEngine simulatedAnnealingEngine)
    {
        var prevTemperature = 0.0;
        var i = 0;
        while (!simulatedAnnealingEngine.IsFinished)
        {
            simulatedAnnealingEngine.NextStep();
            if (i % 100 == 0)
            {
                if (Math.Abs(prevTemperature - simulatedAnnealingEngine.PositionProvider.WorkingTemperature) > 0.000001)
                {
                    prevTemperature = UpdateView(simulatedAnnealingEngine, i);
                }
                await Task.Delay(1).ConfigureAwait(false);
            }
            i++;
        }

        IsRunning = false;
        UpdateView(simulatedAnnealingEngine, i);
    }

    private double UpdateView(SimulatedAnnealingEngine simulatedAnnealingEngine, int i)
    {
        var prevTemperature = simulatedAnnealingEngine.PositionProvider.WorkingTemperature;
        CurrentResult = simulatedAnnealingEngine.Result;
        CurrentSolution = simulatedAnnealingEngine.CurrentSolution;
        History.Add((i, CurrentSolution, CurrentResult));
        CurrentIteration = i;
        InvokeAsync(StateHasChanged);
        return prevTemperature;
    }
}