using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Radzen.Blazor.Rendering;
using ZTR.AI.Example.Shared.ResourceFiles;
using TestContext = Bunit.TestContext;

namespace ZTR.AI.Example.Tests;

public class SimulatedAnnealingPageTests
{
    private TestContext _ctx = null!;

    [SetUp]
    public void Setup()
    {
        _ctx = new Bunit.TestContext();
        _ctx.Services.AddScoped<IStringLocalizerFactory, ResourceManagerStringLocalizerFactory>();
        _ctx.Services.AddScoped<IStringLocalizer<Resource>, StringLocalizer<Resource>>();
    }

    [TearDown]
    public void TearDown()
    {
        _ctx?.Dispose();
    }

    [Test]
    public void FirstBlazor_Test()
    {
        // Arrange
        _ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        _ctx.JSInterop.Setup<Rect>("Radzen.createChart", _ => true);
        var cut = _ctx.RenderComponent<Pages.SimulatedAnnealing>();
        var buttonElement = cut.Find("#startButton");

        // Act
        buttonElement.Click();

        // Assert
        cut.Instance.IsRunning.Should().BeTrue();
    }

    
}