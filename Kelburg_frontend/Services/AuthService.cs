using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Kelburg_frontend.Services;

public class AuthService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly NavigationManager _navigationManager;

    public AuthService(IJSRuntime jsRuntime, NavigationManager navigationManager)
    {
        _jsRuntime = jsRuntime;
        _navigationManager = navigationManager;
    }

    public async Task SetUser(string token, string username)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", token);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "loggedInUser", username);
    }
    
    public async Task<Models.Users?> GetUser(string token = null)
    {
        if (string.IsNullOrEmpty(token))
        {
            token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
        }
        
        if (string.IsNullOrEmpty(token))
        {
            return null;
        }
        
        Dictionary<string, object?> queryParams = new Dictionary<string, object?>()
        {
            { "jwtToken", token }
        };
        
        Models.Users? userLoggedIn = await APIHandler.RequestAPI<Models.Users>(eTables.Users.GetUserFromToken, queryParams, HttpMethod.Get);
        return userLoggedIn;
    }
    
    public async Task<string?> GetToken()
    {
        string? token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
        return token;
    }

    public async Task<bool> IsAdmin(Models.Users userToCheck)
    {
        if (userToCheck != null && userToCheck.AccountType.ToLower().Contains("admin"))
        {
            return true;
        }
        
        return false;
    }
    
    public async Task EnsureAdminAccess()
    {
        Models.Users? userLoggedIn = await GetUser();
        
        if (userLoggedIn == null || !userLoggedIn.AccountType.ToLower().Contains("admin"))
        {
           _navigationManager.NavigateTo("/");
        }
    }
    
    public async Task Logout()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "loggedInUserName");
        _navigationManager.NavigateTo("/login");
    }
}