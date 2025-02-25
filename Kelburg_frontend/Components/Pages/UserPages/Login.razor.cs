using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Kelburg_frontend.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Headers;
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

    [Inject] private IJSRuntime JSRuntime { get; set; }

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
            message = "Logged in as: " + loginResponse.FirstName + " " + loginResponse.LastName;

            string jwtToken = loginResponse.Token;

            Console.WriteLine($"JWT Token: {jwtToken}");
            
            await JSRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", jwtToken);

            await Task.Delay(2000);
        }
        else
        {
            isLogginIn = false;
            message = "Loggin in cancelled";
            StateHasChanged();
        }
    }
}