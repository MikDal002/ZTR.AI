using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ZT.AI.Researcher;

namespace ZTR.AI.Researcher.Greedy;

public static class CommandLineExtension
{
    public static void AddGreedyAlgorithm(this Command command)
    {
        var greedyCommand = new Command("Greedy", "Pozwala na testowanie algorytmu zachłannego")
        {

        };
        greedyCommand.Handler = CommandHandler.Create<GreedyOptions, IHost>(((options, host) =>
            host.Services.GetRequiredService<TesterExecutor<GreedyOptions>>().RunSuite(options)));

        command.AddCommand(greedyCommand);
    }
}