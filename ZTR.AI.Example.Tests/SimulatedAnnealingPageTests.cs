using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Radzen.Blazor.Rendering;
using ZTR.AI.Example.Pages;
using ZTR.AI.Example.Shared.ResourceFiles;
using TestContext = Bunit.TestContext;

namespace ZTR.AI.Example.Tests;

public class EnumerableExtensionsTests
{
    [Test]
    public void CartesianProductMakesCartesianProduct()
    {
        var input = new List<List<double>>
        {
            new List<double>() {1.0, 2.0},
            new List<double>() {-1.0, -2.0}
        };

        var output = input.CreateCartesianProduct();

        output.Should().BeEquivalentTo(new List<double[]>
        {
            new[] {1.0, -1.0}, new[] {1.0, -2.0}, new[] {2.0, -1.0}, new[] {2.0, -2.0}
        });
    }
}

public class SimulatedAnnealingPageTests
{
    private TestContext _ctx = null!;
    private IRenderedComponent<Pages.SimulatedAnnealingPage> _cut;

    [SetUp]
    public void Setup()
    {
        _ctx = new Bunit.TestContext();
        _ctx.Services.AddScoped<IStringLocalizerFactory, ResourceManagerStringLocalizerFactory>();
        _ctx.Services.AddScoped<IStringLocalizer<Resource>, StringLocalizer<Resource>>();
        _ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        _ctx.JSInterop.Setup<Rect>("Radzen.createChart", _ => true);
        _cut = _ctx.RenderComponent<Pages.SimulatedAnnealingPage>();
    }

    [TearDown]
    public void TearDown()
    {
        _ctx?.Dispose();
    }

    [Test]
    public void StartButton_ShouldStartAlgorithm()
    {
        // Arrange
        var buttonElement = _cut.Find("#startButton");

        // Act
        buttonElement.Click();

        // Assert
        _cut.Instance.History!.IsRunning.Should().BeTrue();
    }

    [Test]
    public async Task Algorithm_ShouldStop_WhenFinished()
    {
        // Arrange
        _cut.Instance.EndingTemperature = _cut.Instance.StartingTemperature;
        
        // Act
        await _cut.InvokeAsync(() => _cut.Instance.Start());
        _cut.WaitForState(() => _cut.Instance.History!.IsRunning == false, TimeSpan.FromSeconds(10));

        // Assert
        _cut.Instance.History!.IsRunning.Should().BeFalse();
    }
}