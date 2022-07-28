using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using ZTR.AI.Example.Shared;
using ZTR.AI.Example.Shared.ResourceFiles;
using TestContext = Bunit.TestContext;

namespace ZTR.AI.Example.Tests;

public class MainLayoutTests
{
    private TestContext _ctx;
    private IRenderedComponent<MainLayout> _cut;

    [SetUp]
    public void Setup()
    {
        _ctx = new Bunit.TestContext();
        _ctx.Services.AddScoped<IStringLocalizerFactory, ResourceManagerStringLocalizerFactory>();
        _ctx.Services.AddScoped<IStringLocalizer<Resource>, StringLocalizer<Resource>>();
        _ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        _ctx.JSInterop.SetupModule("./Shared/MainLayout.razor.js");
        _cut = _ctx.RenderComponent<MainLayout>();
    }

    [Test]
    public void MainLayout_MustHaveArticleAttribute()
    {
        var buttonElement = _cut.Find("article");
        buttonElement.Should().NotBeNull();
    }
}