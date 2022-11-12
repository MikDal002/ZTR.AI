using System.Diagnostics;
using MathNet.Numerics.LinearAlgebra;
using ZTR.AI.SimulatedAnnealing.Core;

namespace ZTR.AI.Example.Pages;

public partial class SimulatedAnnealingPage
{
    public IReadOnlyCollection<SingleDimensionalExample> Examples { get; } = SingleDimensionalExample.AllExamples;
    public SingleDimensionalExample CurrentExample { get; private set; } = SingleDimensionalExample.Cos;

    public Vector<double>? CurrentSolution { get; private set; }
    public double CurrentResult { get; private set; }
    public double CurrentIteration { get; private set; }
    public bool IsRunning { get; private set; }
    private List<(int Step, Vector<double> X, double Value)> History { get; } = new();

    public int StartingTemperature { get; set; } = 100;
    public double EndingTemperature { get; set; } = 0.1;

    public async Task Start()
    {
        IsRunning = true;
        History.Clear();

        await PerformAlgorithmAsync().ConfigureAwait(false);
    }

    private async Task PerformAlgorithmAsync()
    {
        var simulatedAnnealingEngine = new SimulatedAnnealingEngine(CurrentExample.Function, StartingTemperature, CurrentExample.Min, CurrentExample.Max, endingTemperature: EndingTemperature);

        var prevTemperature = 0.0;
        var i = 0;
        while (!simulatedAnnealingEngine.IsFinished)
        {
            simulatedAnnealingEngine.NextStep();
            if (i % 100 == 0)
            {
                if (Math.Abs(prevTemperature - simulatedAnnealingEngine.PositionProvider.WorkingTemperature) > 0.000001)
                {
                    prevTemperature = simulatedAnnealingEngine.PositionProvider.WorkingTemperature;
                    UpdateView(simulatedAnnealingEngine, i);
                }
                await Task.Delay(1).ConfigureAwait(false);
            }
            i++;
        }

        IsRunning = false;
        UpdateView(simulatedAnnealingEngine, i);
    }

    private void UpdateView(SimulatedAnnealingEngine simulatedAnnealingEngine, int i)
    {
        CurrentResult = simulatedAnnealingEngine.Result;
        CurrentSolution = simulatedAnnealingEngine.CurrentSolution;
        History.Add((i, CurrentSolution, CurrentResult));
        CurrentIteration = i;
        InvokeAsync(StateHasChanged);
    }
}