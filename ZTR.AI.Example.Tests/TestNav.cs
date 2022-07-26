using AngleSharpWrappers;
using Microsoft.AspNetCore.Components;
using NSubstitute;
using ZTR.AI.Example.Pages;

namespace ZTR.AI.Example.Tests;

internal class TestNav : NavigationManager
{
    public TestNav()
    {
        Initialize("https://unit-test.example/", "https://unit-test.example/");
    }

    protected override void NavigateToCore(string uri, bool forceLoad)
    {
        NotifyLocationChanged(false);
    }
}