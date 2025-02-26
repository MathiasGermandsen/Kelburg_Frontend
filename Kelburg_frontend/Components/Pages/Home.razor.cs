using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;

namespace Kelburg_frontend.Components.Pages;

public partial class Home : ComponentBase
{
    private bool _isInitialized = false;

    protected override void OnInitialized()
    {
        NavigationManager.LocationChanged += OnLocationchanged;
    }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var currentUrl = NavigationManager.Uri;
            if (currentUrl.Contains("/Login"))
            {
                return;
            }
            await JSRuntime.InvokeVoidAsync("localStorage.setItem", "lastPageUrl", currentUrl);
            _isInitialized = true;
        }
    }

    private async void OnLocationchanged(object sender, LocationChangedEventArgs e)
    {
        string jwtToken = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
        
        if (!string.IsNullOrEmpty(jwtToken))
        {
            return;
        }
        
        var currentUrl = e.Location;
        if (currentUrl.Contains("/Login"))
        {
            return;
        }
        await JSRuntime.InvokeVoidAsync("localStorage.setItem", "lastPageUrl", currentUrl);
    }

    public void Dispose()
    {
        NavigationManager.LocationChanged -= OnLocationchanged;
    }
    
}