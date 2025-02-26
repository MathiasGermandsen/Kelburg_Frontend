using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Kelburg_frontend.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Identity.Data;

namespace Kelburg_frontend.Components.Pages.UserPages;

public partial class Login : ComponentBase
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    Users LoggedInUsers = new Users();
    private bool isLogginIn = false;
    private string message = string.Empty;
    private string loggedInUserName = string.Empty;
    [Inject] private IJSRuntime JSRuntime { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }

    private async Task LoggingIn()
    {
        isLogginIn = true;
        message = string.Empty;

        Dictionary<string, object?> queryParams = new()
        {
            { "Email", LoggedInUsers.Email },
            { "Password", LoggedInUsers.PasswordBackdoor },
        };

        var loginRequest = new
        {
            Email = LoggedInUsers.Email,
            Password = LoggedInUsers.PasswordBackdoor
        };

        Console.WriteLine($"Request Data: {JsonSerializer.Serialize(queryParams)}");

        LoginResponse? loginResponse =
            await APIHandler.RequestAPI<LoginResponse>(eTables.Users.Login, queryParams, HttpMethod.Post, loginRequest);

        if (loginResponse != null)
        {
            loggedInUserName = $"{loginResponse.FirstName} {loginResponse.LastName}";
            message = "Logged in as: " + loginResponse.FirstName + " " + loginResponse.LastName;

            string jwtToken = loginResponse.Token;
            Console.WriteLine($"JWT Token: {jwtToken}");

            await JSRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", jwtToken);
            await JSRuntime.InvokeVoidAsync("localStorage.setItem", "loggedInUserName",
                loggedInUserName); // Store name for later use

            var returnUrl = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "lastPageUrl");

            if (!string.IsNullOrEmpty(returnUrl))
            {
                NavigationManager.NavigateTo(returnUrl, forceLoad: true);

                return;
            }
            StateHasChanged();
        }
        else
        {
            isLogginIn = false;
            message = "Email or password is incorrect.";
            StateHasChanged();
        }
    }

    private async Task Logout()
    {
        await JSRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
        await JSRuntime.InvokeVoidAsync("localStorage.removeItem", "loggedInUserName");


        NavigationManager.NavigateTo("/", forceLoad: true);
    }
}