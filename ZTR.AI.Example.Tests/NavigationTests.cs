using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using ZTR.AI.Example.Shared.ResourceFiles;
using TestContext = Bunit.TestContext;

namespace ZTR.AI.Example.Tests;

public class NavigationTests
{
    private TestContext _ctx;
    private IRenderedComponent<NavMenu> _cut;

    [SetUp]
    public void Setup()
    {
        _ctx = new Bunit.TestContext();
        _ctx.Services.AddScoped<IStringLocalizerFactory, ResourceManagerStringLocalizerFactory>();
        _ctx.Services.AddScoped<IStringLocalizer<Resource>, StringLocalizer<Resource>>();
        _cut = _ctx.RenderComponent<NavMenu>();
    }

    [Test]
    public void MenuShouldBeCollapsed_AtStartOfApplication()
    {
        _cut.Instance.NavMenuCssClass.Should().Be("collapse");
    }

    [Test]
    public void MenuShouldBe_Extended_AfterClick()
    {
        // Arrange
        var buttonElement = _cut.Find("#menuCollapser");

        // Act
        buttonElement.Click();

        // Assert
        _cut.Instance.NavMenuCssClass.Should().BeNull();
    }
}