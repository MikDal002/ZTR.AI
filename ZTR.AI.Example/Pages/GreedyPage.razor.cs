using ZTR.AI.Algorithms.Core;

namespace ZTR.AI.Example.Pages;

public partial class GreedyPage
{
    public IEnumerable<SingleDimensionalExample> Examples { get; } = new[]
    {
        SingleDimensionalExample.Sin, SingleDimensionalExample.Cos,  SingleDimensionalExample.ShortExponental,
    };
    public SingleDimensionalExample CurrentExample { get; private set; } = SingleDimensionalExample.Cos;

    public double CurrentSolution { get; private set; }
    public double CurrentResult { get; private set; }
    public double CurrentIteration { get; private set; }
    public bool IsRunning { get; private set; }
    private List<(int Step, double X, double Value)> History { get; } = new();

    public async Task StartGreedy()
    {
        IsRunning = true;
        var greedyEngine = new GreedyEngineForSingleDimensional(CurrentExample.Function, minimumSolutionRange: CurrentExample.Min, maximumSolutionRange: CurrentExample.Max);
        History.Clear();

        await PerformAlgorithm(greedyEngine).ConfigureAwait(false);
    }

    private async Task PerformAlgorithm(GreedyEngineForSingleDimensional greedyEngine)
    {
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

    private void UpdateView(GreedyEngineForSingleDimensional greedyEngine, int i)
    {
        CurrentResult = greedyEngine.Result;
        CurrentSolution = greedyEngine.CurrentSolution;
        History.Add((i, CurrentSolution, CurrentResult));
        CurrentIteration = i;
        InvokeAsync(StateHasChanged);
    }
}