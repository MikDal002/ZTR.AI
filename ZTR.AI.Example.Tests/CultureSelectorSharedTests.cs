using System.Globalization;
using AngleSharp.Html.Dom;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using ZTR.AI.Example.Shared;
using TestContext = Bunit.TestContext;

namespace ZTR.AI.Example.Tests;

public class CultureSelectorSharedTests
{
    private TestContext _ctx = null!;

    [SetUp]
    public void Setup()
    {
        _ctx = new Bunit.TestContext();
        _ctx.Services.AddScoped<NavigationManager, TestNav>();
    }

    [TearDown]
    public void TearDown()
    {
        _ctx?.Dispose();
    }

    [Test]
    public void MustBe_Renderable()
    {
        // Act
        var cut = _ctx.RenderComponent<CultureSelector>();

        // Assert
        cut.Should().NotBeNull();
    }

    [Test]
    public void MustHave_MoreThanOneLanguage()
    {
        // Act
        var cut = _ctx.RenderComponent<CultureSelector>();

        // Assert 
        cut.Instance.Cultures.Should().HaveCountGreaterOrEqualTo(2);
    }

    [Test]
    public void CultureSelector_AllLanguagesAreVisible()
    {
        // Arrange
        var cut = _ctx.RenderComponent<CultureSelector>();

        // Act
        var visibleLanguages = cut.FindAll("option").Select(d => d.InnerHtml);

        // Assert 
        visibleLanguages.Should().Contain(cut.Instance.Cultures.Select(d => d.DisplayName));
    }

    [Test]
    public void CultureSelector_AllLanguagesAreClickable()
    {
        // Arrange
        var cut = _ctx.RenderComponent<CultureSelector>();
        var visibleLanguages = cut.FindAll("option");
        var selector = cut.Find("select");

        // Act
        
        foreach (var language in visibleLanguages.Cast<IHtmlOptionElement>()) 
        {
            var jsRuntimeInvocationHandler = _ctx.JSInterop.SetupVoid("blazorCulture.set", x =>
            {
                x.Arguments.ElementAt(0).Should().Be(language.Value);
                return true;
            });

            jsRuntimeInvocationHandler.SetVoidResult();
            selector.Change(language.Value);
        }
    }


}