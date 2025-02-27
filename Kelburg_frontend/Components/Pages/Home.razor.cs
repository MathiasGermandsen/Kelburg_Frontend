using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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
        string jwtToken = await AuthService.GetToken();
        string currentUrl = e.Location;

        if (currentUrl.Contains("/Login"))
        {
            return;
        }

        if (!string.IsNullOrEmpty(jwtToken))
        {
            if (IsTokenExpired(jwtToken))
            {
                await AuthService.Logout();
            }
        }

        await JSRuntime.InvokeVoidAsync("localStorage.setItem", "lastPageUrl", currentUrl);
    }

    private bool IsTokenExpired(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return true;
        }

        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        JwtSecurityToken jwtToken = tokenHandler.ReadJwtToken(token);

        Claim? expClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "exp");

        if (expClaim == null)
        {
            return true;
        }

        DateTime expDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim.Value)).UtcDateTime;
        return expDate < DateTime.UtcNow;
    }

    public void Dispose()
    {
        NavigationManager.LocationChanged -= OnLocationchanged;
    }
}