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
    private IRenderedComponent<Pages.SimulatedAnnealing> _cut;

    [SetUp]
    public void Setup()
    {
        _ctx = new Bunit.TestContext();
        _ctx.Services.AddScoped<IStringLocalizerFactory, ResourceManagerStringLocalizerFactory>();
        _ctx.Services.AddScoped<IStringLocalizer<Resource>, StringLocalizer<Resource>>();
        _ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        _ctx.JSInterop.Setup<Rect>("Radzen.createChart", _ => true);
        _cut = _ctx.RenderComponent<Pages.SimulatedAnnealing>();
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
        var buttonElement = _cut.Find("#startButton");

        // Act
        buttonElement.Click();

        // Assert
        _cut.Instance.IsRunning.Should().BeTrue();
    }

    [Test]
    public async Task FirstBlazor_Test_ForViewModel()
    {
        // Arrange
        _cut.Instance.EndingTemperature = _cut.Instance.StartingTemperature;
        
        // Act
        await _cut.InvokeAsync(() => _cut.Instance.StartSimulatedAnnealing());
        _cut.WaitForState(() => _cut.Instance.IsRunning == false, TimeSpan.FromSeconds(10));

        // Assert
        _cut.Instance.IsRunning.Should().BeFalse();
    }

}