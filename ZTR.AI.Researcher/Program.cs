using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.NamingConventionBinder;
using System.CommandLine.Parsing;
using ZTR.AI.Researcher.GA;
using ZTR.AI.Researcher.Greedy;
using ZTR.AI.Researcher.SimulatedAnnealing;
using ZTR.AI.Researcher.Tabu;

namespace ZT.AI.Researcher;

class Program
{
    static async Task Main(string[] args) => await BuildCommandLine()
        .UseHost(_ => Host.CreateDefaultBuilder(),
            host =>
            {
                host.ConfigureServices(services =>
                {
                    services.AddTransient<ITester<GreedyOptions>, GreedyTester>();
                    services.AddTransient<ITester<SimulatedAnnealingOptions>, SimulatedAnnealingTester>();
                    services.AddTransient<ITester<TabuOptions>, TabuTester>();
                    services.AddTransient<ITester<GenethicsAlgorithmsOptions>, GenethicsAlgorithmTester>();
                    services.AddTransient<TestFunctionProvider>();
                    services.AddTransient(typeof(TesterExecutor<>));
                });
            })
        .UseDefaults()
        .Build()
        .InvokeAsync(args);

    private static CommandLineBuilder BuildCommandLine()
    {
        var root = new RootCommand(@"$ dotnet run --name 'Joe'");

        root.AddTabuAlgorithm();
        root.AddGreedyAlgorithm();
        root.AddSimulatedAnnealingAlgorithm();
        root.AddGenethicsAlgorithm();

        root.AddGlobalOption(new Option<FileInfo>("--Output", "Określa plik, do którego zostaną zapisane dane")
        {
            IsRequired = true
        });

        root.AddGlobalOption(new Option<int>("--Repeat", "Określa ile razy powtórzyć wykonania")
        {
            IsRequired = true
        });
        root.AddGlobalOption(new Option<int>("--StepsAtEnd", "Ilość kroków na końcu")
        {
            IsRequired = true
        });

        root.AddGlobalOption(new Option<int>("--StepsAtBeginning", "Ilość kroków na poczatku")
        {
            IsRequired = true
        });
        root.AddGlobalOption(new Option<TestFunction>("--TestFunction", "Funkcja, która zostanie poddana testom")
        {
            IsRequired = true
        }
        );

        return new CommandLineBuilder(root);
    }
}