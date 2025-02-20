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
            { "FirstName", newUser.FirstName },
            { "LastName", newUser.LastName },
            { "PhoneNumber", newUser.PhoneNumber },
            { "Address", newUser.Address },
            { "City", newUser.City },
            { "Country", newUser.Country },
            { "PostalCode", newUser.PostalCode },
            { "Email", newUser.Email },
            { "Password", newUser.PasswordBackdoor },
            { "AccountType", "user" },
        };
        
        try
        {
            List<Users> userCreated = await APIHandler.RequestAPI<List<Users>>(eTables.Users.Create, queryParams, HttpMethod.Post);

            if (userCreated.Any())
            {
                message = "Account created successfully";
                await Task.Delay(2000);
                NavigationManager.NavigateTo("/UserPages/UserPages/");
            }
            else
            {
                message = "Account creation failed: nu user data returned";
            }
        }
        catch (HttpRequestException httpException)
        {
            message = $"HTTP Error: {httpException.StatusCode}-{httpException.Message}";
        }
        catch (Exception ex)
        {
            message = $"Error: {ex.Message}";
        }
        finally
        {
            isSubmitting = false;
        }
    }
    private void GoBack()
    {
        NavigationManager.NavigateTo("/UserPages/UserPages/");
    }
}