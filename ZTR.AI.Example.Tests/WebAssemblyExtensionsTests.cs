using System.Globalization;
using FluentAssertions;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using NSubstitute;

namespace ZTR.AI.Example.Tests;

public class WebAssemblyExtensionsTests
{
    [Test]
    public void SetDefaultCultureAsync_ThrowsArgumentNullException_WhenJSRUntimeArgumentIsNull()
    {
        // Act
        var action = () => WebAssemblyHostExtension.SetDefaultCultureAsync((IJSRuntime) null!);
        action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public void SetDefaultCultureAsync_ThrowsArgumentNullException_WhenWebHostArgumentIsNull()
    {
        // Act
        var action = () => WebAssemblyHostExtension.SetDefaultCultureAsync((WebAssemblyHost)null!);
        action.Should().ThrowAsync<ArgumentNullException>();
    }

    [TestCase(null, "pl-PL")]
    [TestCase("pl-PL", "pl-PL")]
    [TestCase("en-Us", "en-US")]
    public async Task DefaultCulture_IsPlpl(string input, string output)
    {
        var jsRuntime = Substitute.For<IJSRuntime>();
        jsRuntime.InvokeAsync<string>(Arg.Any<string>())!.Returns(input);

        await WebAssemblyHostExtension.SetDefaultCultureAsync(jsRuntime);

        var cultureInfo = new CultureInfo(output);
        CultureInfo.DefaultThreadCurrentCulture.Should().Be(cultureInfo);
        CultureInfo.DefaultThreadCurrentUICulture.Should().Be(cultureInfo);
    }
}