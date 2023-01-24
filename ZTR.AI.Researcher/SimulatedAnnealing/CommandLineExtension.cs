using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ZT.AI.Researcher;

namespace ZTR.AI.Researcher.SimulatedAnnealing;

public static class CommandLineExtension
{
    public static void AddSimulatedAnnealingAlgorithm(this Command command)
    {
        var simulatedAnnealingCommand = new Command("SA", "Pozwala na testowanie algorytmu symulowane wyżarzania")
        {
            new Option<double>("--StartingTemperature", "Określa startową temperaturę algorytmu")
        };
        simulatedAnnealingCommand.Handler = CommandHandler.Create<SimulatedAnnealingOptions, IHost>((options, host) =>
        {
            options.AlgorithmName = "SA";
            host.Services.GetRequiredService<TesterExecutor<SimulatedAnnealingOptions>>().RunSuite(options);
        });


        command.AddCommand(simulatedAnnealingCommand);
    }
}