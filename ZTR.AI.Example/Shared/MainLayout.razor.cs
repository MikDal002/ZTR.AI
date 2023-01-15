using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ZTR.AI.Example.Shared;

public partial class MainLayout
{
    [Inject] private IJSRuntime JsRuntime { get; set; } = default!;
    
    public string Title { get; set; } = "";

    protected override async Task OnAfterRenderAsync(bool firstRender)

    {
        var module = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "./Shared/MainLayout.razor.js");

        var newTitle = await module.InvokeAsync<string>("getTitle");
        if (newTitle != Title)
        {
            Title = newTitle;
            StateHasChanged();
        }
        await base.OnAfterRenderAsync(firstRender);
    }
}