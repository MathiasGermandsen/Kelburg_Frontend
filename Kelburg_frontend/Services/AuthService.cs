using Microsoft.JSInterop;

namespace Kelburg_frontend.Services;

public class AuthService
{
    private readonly IJSRuntime _jsRuntime;

    public AuthService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task SetUser(string token, string username)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", token);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "loggedInUserName", username);
    }

    public async Task<string> GetUserName()
    {
        return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "loggedInUserName");
    }

    public async Task Logout()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "loggedInUserName");
    }
}