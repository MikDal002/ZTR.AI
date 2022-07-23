using System.Runtime.ExceptionServices;
using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace ZTR.AI.Examples.Tests;

public class OtherTests
{
    [Test]
    public void METHOD()
    {
        var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                // ... Configure test services
            });
        var client = application.CreateClient();
    }
}

public abstract class WebHostServerFixture : IDisposable
{
    private readonly Lazy<Uri> _rootUriInitializer;

    public Uri RootUri => _rootUriInitializer.Value;
    public IHost Host { get; set; }

    public WebHostServerFixture()
    {
        _rootUriInitializer = new Lazy<Uri>(() => new Uri(StartAndGetRootUri()));
    }

    protected virtual string StartAndGetRootUri()
    {
        Host = CreateWebHost();
        RunInBackgroundThread(Host.Start);
        return Host.Services.GetService<IServer>().Features
            .Get<IServerAddressesFeature>()
            .Addresses.Single();
    }

    protected abstract IHost CreateWebHost();

    protected static void RunInBackgroundThread(Action action)
    {
        using var isDone = new ManualResetEvent(false);
        ExceptionDispatchInfo edi = null;
        new Thread(() =>
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                edi = ExceptionDispatchInfo.Capture(ex);
            }

            isDone.Set();
        }).Start();

        if (!isDone.WaitOne(TimeSpan.FromSeconds(10))) throw new TimeoutException("Timed out waiting for: " + action);

        if (edi != null)
        {
            edi.Throw();
        }
    }

    public virtual void Dispose()
    {
        Host?.Dispose();
        Host?.StopAsync();
    }
}

public class BlazorWebAssemblyWebHostFixture<TProgram> : WebHostServerFixture
{
    protected override IHost CreateWebHost()
    {
        return new HostBuilder()
            .ConfigureHostConfiguration(config =>
            {
                var applicationPath = typeof(TProgram).Assembly.Location;
                var applicationDirectory = Path.GetDirectoryName(applicationPath);

#if NET6_0_OR_GREATER
                var name = Path.ChangeExtension(applicationPath, ".staticwebassets.runtime.json");
#else
                var name = Path.ChangeExtension(applicationPath, ".StaticWebAssets.xml");
#endif
                var inMemoryConfiguration = new Dictionary<string, string>()
                {
                    [WebHostDefaults.StaticWebAssetsKey] = name
                };
            })
            .ConfigureWebHostDefaults(webBuilder => webBuilder
                .UseKestrel()
                .UseSolutionRelativeContentRoot(typeof(TProgram).Assembly.GetName().Name)
                .UseStaticWebAssets()
                .UseStartup<Startup>()
                .UseUrls($"http://127.0.01:0"))
            .Build();
    }

    private sealed class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles(new StaticFileOptions() {ServeUnknownFileTypes = true});
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}

[Parallelizable(ParallelScope.Self)]
public class SimplePlaywrightTests
{
    private readonly BlazorWebAssemblyWebHostFixture<Startup> _server;

    [SetUp]
    public void Setup()
    {
        var exitCode = Microsoft.Playwright.Program.Main(new[] { "install" });
        if (exitCode != 0)
        {
            throw new Exception($"Playwright exited with code {exitCode}");
        }
    }

    [Test]
    public async Task Test1()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Firefox.LaunchAsync(new BrowserTypeLaunchOptions()
        {
            Headless = false, SlowMo = 500
        });
        var page = await browser.NewPageAsync();
        await page.GotoAsync("https://playwright.dev/dotnet");
        await page.ScreenshotAsync(new PageScreenshotOptions() {Path = "sceenshot.png"});
    }

    [Test]
    public async Task METHOD()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false,
        });
        var context = await browser.NewContextAsync();
        // Open new page
        var page = await context.NewPageAsync();
        // Go to https://localhost:7204/
        await page.GotoAsync("https://localhost:7204/");
        // Click text=Symulowane wy¿arzanie
        await page.Locator("text=Symulowane wy¿arzanie").ClickAsync();
        await page.WaitForURLAsync("https://localhost:7204/SimulatedAnnealing");
        // Click button:has-text("Start")
        await page.Locator("button:has-text(\"Start\")").ClickAsync();
        // Click text=Wartoœæ bie¿¹cego rozwi¹zania (wartoœæ y): -0,9997049715632013
        await page.Locator("text=Wartoœæ bie¿¹cego rozwi¹zania (wartoœæ y): -0,9997049715632013").ClickAsync();
    }
}

[Parallelizable(ParallelScope.Self)]
public class PagePlaywrightTests : PageTest
{
    [Test]
    public async Task ShouldAdd()
    {
        int result = await Page.EvaluateAsync<int>("() => 7 + 3");
        result.Should().Be(10);
    }

    [Test]
    public async Task ShouldMultiply()
    {
        int result = await Page.EvaluateAsync<int>("() => 7 * 3");
        result.Should().Be(21);
    }

    
}