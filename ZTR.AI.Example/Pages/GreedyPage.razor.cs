using MathNet.Numerics.LinearAlgebra;
using ZTR.AI.Algorithms.Core;

namespace ZTR.AI.Example.Pages;

public partial class GreedyPage
{

    public IReadOnlyCollection<SingleDimensionalExample> Examples { get; } = SingleDimensionalExample.AllExamples;
    public SingleDimensionalExample CurrentExample { get; private set; } = SingleDimensionalExample.Cos;

    public Vector<double>? CurrentSolution { get; private set; }
    public double CurrentResult { get; private set; }
    public double CurrentIteration { get; private set; }
    public bool IsRunning { get; private set; }
    private List<(int Step, Vector<double> X, double Value)> History { get; } = new();

    public async Task Start()
    {
        IsRunning = true;
        History.Clear();

        await PerformAlgorithm().ConfigureAwait(false);
    }

    private async Task PerformAlgorithm()
    {
        var greedyEngine = new GreedyEngine(CurrentExample.Function, minimumSolutionRange: CurrentExample.Min, maximumSolutionRange: CurrentExample.Max);

        var i = 0;
        while (!greedyEngine.IsFinished)
        {
            greedyEngine.NextStep();
            if (i % 100 == 0)
            {
                UpdateView(greedyEngine, i);

                await Task.Delay(1).ConfigureAwait(false);
            }

            i++;
        }

        IsRunning = false;
        UpdateView(greedyEngine, i);
    }

    private void UpdateView(GreedyEngine greedyEngine, int i)
    {
        CurrentResult = greedyEngine.Result;
        CurrentSolution = greedyEngine.CurrentSolution;
        History.Add((i, CurrentSolution, CurrentResult));
        CurrentIteration = i;
        InvokeAsync(StateHasChanged);
    }
}