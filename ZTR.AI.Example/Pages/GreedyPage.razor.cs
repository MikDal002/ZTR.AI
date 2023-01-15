using Light.GuardClauses;
using Microsoft.VisualBasic;
using ZTR.AI.Algorithms.Core;

namespace ZTR.AI.Example.Pages;

public partial class GreedyPage
{

    public IReadOnlyCollection<TestFunction> Examples { get; } = TestFunction.AllExamples;
    public TestFunction CurrentExample { get; private set; } = TestFunction.Cos;

    public ExampleHistory? History { get; private set; }

    public async Task Start()
    {
        History = new ExampleHistory();
        History.Start();

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

        History?.Stop();
        UpdateView(greedyEngine, i);
    }

    private void UpdateView(GreedyEngine greedyEngine, int i)
    {
        History!.Update(greedyEngine.Result, greedyEngine.CurrentSolution, i);
        InvokeAsync(StateHasChanged);
    }
}