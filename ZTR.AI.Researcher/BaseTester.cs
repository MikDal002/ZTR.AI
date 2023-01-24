using MathNet.Numerics.Statistics;
using Microsoft.Extensions.Logging;

namespace ZT.AI.Researcher;

public abstract class BaseTester<T> where T : IGlobalOptions 
{
    private readonly ILogger<T> _logger;
    private readonly RawResultsHandler _handler;

    protected BaseTester(ILogger<T> logger, RawResultsHandler handler)
    {
        _logger = logger;
        _handler = handler;
    }

    public void Run(T options)
    {
        using var fileStream = File.OpenWrite(options.Output.FullName);
        using var writer = new StreamWriter(fileStream);

        int currentSteps = options.StepsAtBeginning;

        writer.WriteLine($"Steps;Avg;StdDev;Min;Max;Count");

        while (currentSteps < options.StepsAtEnd)
        {
            _logger.LogInformation($"Started testing for {currentSteps}...");

            var results = MakeExamination(options, currentSteps, writer);
            _handler.SaveResults(options, currentSteps, results);

            currentSteps *= 2;
        }

        _logger.LogInformation("Test finished...");
    }

    private IEnumerable<double> MakeExamination(T options, int currentSteps, StreamWriter writer)
    {
        List<double> results = new(options.Repeat);

        for (int i = 0; i < options.Repeat; ++i)
        {
            var (result, steps) = RunInternal(options, currentSteps);
            results.Add(result);
        }

        writer.WriteLine(
            $"{currentSteps};" +
            $"{results.Average().ToString("F5")};" +
            $"{results.StandardDeviation().ToString("F5")};" +
            $"{results.Min().ToString("F5")};" +
            $"{results.Max().ToString("F5")};" +
            $"{results.Count}"
            );
        return results;
    }

    public abstract (double Result, int Steps) RunInternal(T options, int steps);
}