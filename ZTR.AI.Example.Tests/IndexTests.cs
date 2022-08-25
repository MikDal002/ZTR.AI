using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using ZTR.AI.Example.Shared.ResourceFiles;
using TestContext = Bunit.TestContext;

namespace ZTR.AI.Example.Tests;

public class IndexTests
{
    private TestContext _ctx;
    private IRenderedComponent<Pages.Index> _cut;

    [SetUp]
    public void Setup()
    {
        _ctx = new Bunit.TestContext();
        _ctx.Services.AddScoped<IStringLocalizerFactory, ResourceManagerStringLocalizerFactory>();
        _ctx.Services.AddScoped<IStringLocalizer<Resource>, StringLocalizer<Resource>>();
        _cut = _ctx.RenderComponent<Pages.Index>();
    }

    [TearDown]
    public void TearDown()
    {
        _ctx.Dispose();
        _cut.Dispose();
    }

    [Test]
    public void IndexPage_ShouldHaveTitle()
    {
        // Arrange 
        var resources = _ctx.Services.GetRequiredService<IStringLocalizer<Resource>>();

        // Act
        var buttonElement = _cut.Find("h1");

        // Assert
        buttonElement.InnerHtml.Should().Contain(resources["helloworld"]);
    }
}