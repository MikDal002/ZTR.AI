using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ZT.AI.Researcher;

namespace ZTR.AI.Researcher.Tabu;

public static class CommandLineExtension
{
    public static void AddTabuAlgorithm(this Command command)
    {
        var tabuCommand = new Command("Tabu", "Pozwala na testowanie algorytmu symulowane wyżarzania")
        {
            new Option<double>("--TabuRange", "Określa wielkość tabu"),
            new Option<double>("--RestartsAmount", "Określa ilość restartów algorytmu"),
            new Option<double>("--ExplorationAmount", "Określa jak długo algorytm ma eksplorowac dane miejsce"),
            new Option<double>("--TabuListMax", "Określa długość listy tabu"),
        };
        tabuCommand.Handler = CommandHandler.Create<TabuOptions, IHost>(((options, host) =>
            host.Services.GetRequiredService<TesterExecutor<TabuOptions>>().RunSuite(options)));

        command.AddCommand(tabuCommand);
    }
}