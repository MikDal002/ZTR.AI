using MathNet.Numerics.Statistics;
using Microsoft.Extensions.Logging;

namespace ZT.AI.Researcher;

public abstract class BaseTester<T> where T : IGlobalOptions 
{
    private readonly ILogger<T> _logger;

    protected BaseTester(ILogger<T> logger)
    {
        _logger = logger;
    }

    public void Run(T options)
    {
        _logger.LogInformation("Started testing...");
        using var fileStream = File.OpenWrite(options.Output.FullName);
        using var writer = new StreamWriter(fileStream);

        int currentSteps = options.StepsAtBeginning;

        writer.WriteLine($"Steps;Min;Max;Avg;StdDev;Count");

        while (currentSteps < options.StepsAtEnd)
        {
            MakeExamination(options, currentSteps, writer);
            currentSteps *= 2;
        }
        _logger.LogInformation("Test finished...");
    }

    private void MakeExamination(T options, int currentSteps, StreamWriter writer)
    {
        List<double> results = new(options.Repeat);

        for (int i = 0; i < options.Repeat; ++i)
        {
            var (result, steps) = RunInternal(options, currentSteps);
            results.Add(result);
        }

        writer.WriteLine(
            $"{currentSteps};{results.Min().ToString("F5")};" +
            $"{results.Max().ToString("F5")};" +
            $"{results.Average().ToString("F5")};" +
            $"{results.StandardDeviation().ToString("F5")};" +
            $"{results.Count}");
    }

    public abstract (double Result, int Steps) RunInternal(T options, int steps);
}