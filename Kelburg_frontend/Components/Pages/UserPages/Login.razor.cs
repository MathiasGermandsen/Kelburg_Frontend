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
    Users UserLoggedIn = new Users();
    
    private bool isLogginIn = false;
    
    private string message = string.Empty;
    
    string inputEmail = string.Empty;
    string inputPassword = string.Empty;
    
    private async Task LogIn()
    {
        isLogginIn = true;
        message = string.Empty;
        
        Dictionary<string, object?> queryParams = new Dictionary<string, object?>()
        {
            {"email", inputEmail},
            {"password", inputPassword}
        };
        
        string jwtToken = await APIHandler.RequestAPI<string>(eTables.Users.Login, queryParams, HttpMethod.Post);
        
        if (jwtToken != null)
        {
            UserLoggedIn = await AuthService.GetUser(jwtToken);
            
            if (UserLoggedIn == null)
            {
                return;
            }
            
            await AuthService.SetUser(jwtToken, $"{UserLoggedIn.FirstName} {UserLoggedIn.LastName}");

            if (await AuthService.IsAdmin(UserLoggedIn))
            {
                NavigationManager.NavigateTo("/staffLandingPage");
            }
            else
            {
                string returnUrl = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "lastPageUrl");

                if (!string.IsNullOrEmpty(returnUrl))
                {
                    NavigationManager.NavigateTo(returnUrl, forceLoad: true);
                }
            }
        }
        else
        {
            isLogginIn = false;
            message = "Email or password is incorrect.";
            StateHasChanged();
        }
    }
}