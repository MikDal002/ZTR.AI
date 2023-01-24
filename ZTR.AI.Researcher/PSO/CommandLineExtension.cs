using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using ZT.AI.Researcher;

namespace ZTR.AI.Researcher.PSO;

public static class CommandLineExtension
{
    public static void AddParticleSwarmOptimisationAlgorithm(this Command command)
    {
        var gaCommand = new Command("PSO", "Pozwala na testowanie algorytmu optymalizacji rojem cząstek")
        {

        };
        gaCommand.Handler = CommandHandler.Create<ParticleSwarmOptimisationAlgorithmsOptions, IHost>((options, host) =>
        {
            options.AlgorithmName = "PSO";
            host.Services.GetRequiredService<TesterExecutor<ParticleSwarmOptimisationAlgorithmsOptions>>()
                .RunSuite(options);
        });

        command.AddCommand(gaCommand);
    }
}