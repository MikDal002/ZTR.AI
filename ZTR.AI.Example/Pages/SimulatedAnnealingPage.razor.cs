using System.Diagnostics;
using MathNet.Numerics.LinearAlgebra;
using ZTR.AI.Algorithms.Core;
using ZTR.AI.SimulatedAnnealing.Core;

namespace ZTR.AI.Example.Pages;

public partial class SimulatedAnnealingPage
{
    public IReadOnlyCollection<SingleDimensionalExample> Examples { get; } = SingleDimensionalExample.AllExamples;
    public SingleDimensionalExample CurrentExample { get; private set; } = SingleDimensionalExample.Cos;

    public ExampleHistory? History { get; private set; }

    public int StartingTemperature { get; set; } = 100;
    public double EndingTemperature { get; set; } = 0.1;

    public async Task Start()
    {
        History = new ExampleHistory();
        History.Start();

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

        History?.Stop();
        UpdateView(simulatedAnnealingEngine, i);
    }

    private void UpdateView(SimulatedAnnealingEngine simulatedAnnealingEngine, int i)
    {

        History!.Update(simulatedAnnealingEngine.Result, simulatedAnnealingEngine.CurrentSolution, i);
        InvokeAsync(StateHasChanged);
    }
}