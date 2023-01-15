using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.NamingConventionBinder;
using System.CommandLine.Parsing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
                });
            })
        .UseDefaults()
        .Build()
        .InvokeAsync(args);

    private static CommandLineBuilder BuildCommandLine()
    {
        var root = new RootCommand(@"$ dotnet run --name 'Joe'"){
            new Option<string>("--name"){
                IsRequired = true
            }
        };
        root.Handler = CommandHandler.Create<GreeterOptions, IHost>(Run);
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
interface IGreeter
{
    void Greet(string name) => Console.WriteLine($"Hello, {name ?? "anonymous"}");
}

public class Greeter : IGreeter
{

}
public class HostingPlaygroundLogEvents
{
    public const int GreetEvent = 1000;
}
public class GreeterOptions
{
    public string Name { get; }

    public GreeterOptions(string name)
    {
        Name = name;
    }
}