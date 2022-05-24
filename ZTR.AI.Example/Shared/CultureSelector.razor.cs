using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ZTR.AI.Example.Shared
{
    public partial class CultureSelector
    {

        [Inject] public NavigationManager NavigationManager { get; set; } = default!;

        [Inject] public IJSRuntime JSRuntime { get; set; } = default!;

        private CultureInfo[] cultures = new[]
        {
            new CultureInfo("pl-PL"),
            new CultureInfo("en-US"),
        };

        CultureInfo Culture
        {
            get => CultureInfo.CurrentCulture;
            set
            {
                if (CultureInfo.CurrentCulture != value)
                {
                    var js = (IJSInProcessRuntime)JSRuntime;
                    js.InvokeVoid("blazorCulture.set", value.Name);

                    NavigationManager.NavigateTo(NavigationManager.Uri, forceLoad: true);
                }
            }
        }
    }
}
