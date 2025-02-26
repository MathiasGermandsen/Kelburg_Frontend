using Kelburg_frontend.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Kelburg_frontend.Components.Pages.AdminPages;

public partial class Users : ComponentBase
{
    private string FirstName = "";
    private string LastName = "";
    private int UserId;
    private List<Models.Users>? UsersList = new List<Models.Users>();
    
    private int pageSize = 12;
    private int pageNumber = 1;

    private bool isSearching = false;
    private bool _hasRun = false;
    
    private Models.Users UserLoggedIn = new Models.Users();
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !_hasRun)
        {
            _hasRun = true;

            isSearching = true;
            StateHasChanged();

            await AuthService.VerifyAdmin();
            UserLoggedIn = await AuthService.GetUser();
            await SearchUsers(pageSize, 1);

            StateHasChanged();
        }
    }


    private async Task SearchClicked()
    {
        await SearchUsers(pageSize, 1);
    }

    private async Task SearchUsers(int pageSize, int pageNumber)
    {
        UsersList.Clear();
        
        Dictionary<string, object?> queryParams = new Dictionary<string, object?>()
        {
            {"pageSize", pageSize},
            {"pageNumber", pageNumber},
        };

        if (!string.IsNullOrEmpty(FirstName))
        {
            queryParams.Add("firstName", FirstName);
        }
        
        if (!string.IsNullOrEmpty(LastName))
        {
            queryParams.Add("lastName", LastName);
        }
        
        if (UserId != 0 && UserId != null)
        {
            queryParams.Add("userId", UserId);
        }
        isSearching = true;
        
        UsersList = await APIHandler.RequestAPI<List<Models.Users>>(eTables.Users.Read, queryParams, HttpMethod.Get, null, await AuthService.GetToken());
        await AddDelay(350, 750); 
        
        isSearching = false;
    }
    
    private async Task NextPage()
    {
        pageNumber++;
        await SearchUsers(pageSize, pageNumber);
    }

    private async Task PreviousPage()
    {
        pageNumber--;
        await SearchUsers(pageSize, pageNumber);
    }

    private async Task ClickRedirect(Models.Users user)
    {
        NavigationManager.NavigateTo($"/user/{user.Id}");
    }
    
    private async Task AddDelay(int min, int max)
    {
        Random random = new Random();
        Thread.Sleep(random.Next(min, max));
    }
}