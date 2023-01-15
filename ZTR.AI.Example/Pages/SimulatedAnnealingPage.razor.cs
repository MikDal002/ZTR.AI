using System.Runtime.CompilerServices;
using Light.GuardClauses;
using Plotly.Blazor;
using Plotly.Blazor.Traces;
using Plotly.Blazor.Traces.ScatterLib;
using ZTR.AI.SimulatedAnnealing.Core;

namespace ZTR.AI.Example.Pages;

public partial class SimulatedAnnealingPage
{
    public IReadOnlyCollection<TestFunction> Examples { get; } = TestFunction.AllExamples;



    public ExampleHistory? History { get; private set; }

    public int StartingTemperature { get; set; } = 100;
    public double EndingTemperature { get; set; } = 0.1;

    private async Task UpdateTestFunctionChart(TestFunction function)
    {
        chart.MustNotBeNull();
        if (chart is null) throw new NotImplementedException();

        await chart.Clear();
        if (function.Dimensions == 1)
        {
            var scatter = new Scatter()
            {
                Name = "ScatterTrace",
                Mode = ModeFlag.Lines | ModeFlag.Markers,
                X = function.DrawingData.Select(d => (object)d.X[0]).ToList(),
                Y = function.DrawingData.Select(d => (object)d.Y).ToList()
        };
            await chart.AddTrace(scatter);
        }
        else if (function.Dimensions == 2)
        {
            var objects = function.DrawingData.Select(d => (object) d.X[0]).ToList();
            var list = function.DrawingData.Select(d => (object) d.X[1]).ToList();
            var objects1 = function.DrawingData.Select(d => (object) d.Y).ToList();
            var surface = new Surface()
            {
                Name = "Surface",

                X = objects,
                Z = list, 
                Y = objects1

            };
            await chart.AddTrace(surface);
        }

        //await chart.React();
    }

    public TestFunction CurrentExample
    {
        get => _currentExample;
        private set
        {
            if (_currentExample != value)
            {
                _currentExample = value;
                UpdateTestFunctionChart(value).ConfigureAwait(false);
            }
        }
    }

    PlotlyChart? chart;
    Config config = new Config()
    {
        Responsive = true
    };
    Layout layout = new Layout();
    // Using of the interface IList is important for the event callback!
    IList<ITrace> data = new List<ITrace>
    {
      new Surface()
      {
          Z = new List<object>()
              {
                  new List<object>{1,1},
                  new List<object>{3,4},
              },
          Name = "Test"
      } 
    };

    private TestFunction _currentExample = TestFunction.Cos;

    public SimulatedAnnealingPage()
    {
        UpdateTestFunctionChart(TestFunction.Sum).ConfigureAwait(false);
    }

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