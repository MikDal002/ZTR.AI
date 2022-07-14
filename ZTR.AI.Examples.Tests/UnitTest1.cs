using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Testing;
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

[Parallelizable(ParallelScope.Self)]
public class SimplePlaywrightTests
{
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