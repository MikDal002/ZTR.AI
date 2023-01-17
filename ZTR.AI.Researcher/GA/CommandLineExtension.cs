using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ZT.AI.Researcher;

namespace ZTR.AI.Researcher.GA;

public static class CommandLineExtension
{
    public static void AddGenethicsAlgorithm(this Command command)
    {
        var gaCommand = new Command("GA", "Pozwala na testowanie algorytmu genetycznego")
        {

        };
        gaCommand.Handler = CommandHandler.Create<GenethicsAlgorithmsOptions, IHost>(((options, host) =>
            host.Services.GetRequiredService<TesterExecutor<GenethicsAlgorithmsOptions>>().RunSuite(options)));

        command.AddCommand(gaCommand);
    }
}