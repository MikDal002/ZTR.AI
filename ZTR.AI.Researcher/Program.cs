using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.NamingConventionBinder;
using System.CommandLine.Parsing;

namespace ZT.AI.Researcher;

class Program
{
    static async Task Main(string[] args) => await BuildCommandLine()
        .UseHost(_ => Host.CreateDefaultBuilder(),
            host =>
            {
                host.ConfigureServices(services =>
                {
                    services.AddSingleton<IGreeter, Greeter>();
                    services.AddTransient<ITester<GreedyOptions>, GreedyTester>();
                    services.AddTransient<ITester<SimulatedAnnealingOptions>, SimulatedAnnealingTester>();
                    services.AddTransient<TestFunctionProvider>();
                    services.AddTransient(typeof(TesterExecutor<>));
                });
            })
        .UseDefaults()
        .Build()
        .InvokeAsync(args);

    private static CommandLineBuilder BuildCommandLine()
    {
        var greedyCommand = new Command("Greedy", "Pozwala na testowanie algorytmu zachłannego")
        {

        };
        greedyCommand.Handler = CommandHandler.Create<GreedyOptions, IHost>(((options, host) =>
            host.Services.GetRequiredService<TesterExecutor<GreedyOptions>>().RunSuite(options)));

        var simulatedAnnealingCommand = new Command("SA", "Pozwala na testowanie algorytmu symulowane wyżarzania")
        {
            new Option<double>("--StartingTemperature", "Określa startową temperaturę algorytmu")
        };
        simulatedAnnealingCommand.Handler = CommandHandler.Create<SimulatedAnnealingOptions, IHost>(((options, host) =>
            host.Services.GetRequiredService<TesterExecutor<SimulatedAnnealingOptions>>().RunSuite(options)));

        var root = new RootCommand(@"$ dotnet run --name 'Joe'"){
            new Option<string>("--name"){
                IsRequired = true
            },

            greedyCommand,
            simulatedAnnealingCommand
        };
        root.Handler = CommandHandler.Create<GreeterOptions, IHost>(Run);
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

    private static void Run(GreeterOptions options, IHost host)
    {
        var serviceProvider = host.Services;
        var greeter = serviceProvider.GetRequiredService<IGreeter>();
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger(typeof(Program));

        var name = options.Name;
        logger.LogInformation(HostingPlaygroundLogEvents.GreetEvent, "Greeting was requested for: {name}", name);
        greeter.Greet(name);
    }
}