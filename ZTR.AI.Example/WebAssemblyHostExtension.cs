using System.Globalization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;

namespace ZTR.AI.Example;

public static class WebAssemblyHostExtension
{
    public static async Task SetDefaultCultureAsync(IJSRuntime jsInterop)
    {
        ArgumentNullException.ThrowIfNull(jsInterop);

        var result = await jsInterop.InvokeAsync<string>("blazorCulture.get").ConfigureAwait(false);

        CultureInfo culture;
        if (!string.IsNullOrWhiteSpace(result)) culture = new CultureInfo(result);
        else culture = new CultureInfo("pl-PL");

        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }

    public static async Task SetDefaultCultureAsync(this WebAssemblyHost host)
    {
        ArgumentNullException.ThrowIfNull(host);

        var jsInterop = host.Services.GetRequiredService<IJSRuntime>();
        await SetDefaultCultureAsync(jsInterop).ConfigureAwait(false);
    }
}