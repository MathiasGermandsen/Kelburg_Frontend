using Microsoft.AspNetCore.Components;
using Kelburg_frontend.Models;

namespace Kelburg_frontend.Components.Pages.UserPages;

public partial class SignUp : ComponentBase
{
    Users newUser = new Users();
    private bool isSubmitting = false;
    private string message = string.Empty;

    private async Task CreateAccount()
    {
        isSubmitting = true;
        message = "string.Empty";
        
        Dictionary<string, object?> queryParams = new()
        {
            {"FirstName", newUser.FirstName},
            {"LastName", newUser.LastName},
            {"PhoneNumber", newUser.PhoneNumber},
            {"Address", newUser.Address},
            {"City", newUser.City},
            {"Country", newUser.Country},
            {"PostalCode", newUser.PostalCode},
            {"Email", newUser.Email},
            {"PasswordBackdoor", newUser.PasswordBackdoor},
            {"AccountType", "user"},
        };
        
        Users userCreated = await APIHandler.RequestAPI<Users>(eTables.Users.Create, queryParams, HttpMethod.Post);

        if (userCreated != null)
        {
            message = "Account created successfully";
            await Task.Delay(2000);
            NavigationManager.NavigateTo("/UserPages/UserPages/Home");
        }
        else
        {
            message = "Account creation failed";
        }
        
        isSubmitting = false;
    }

    private void GoBack()
    {
        NavigationManager.NavigateTo("/UserPages/UserPages/Home");
    }
}