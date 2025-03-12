using Microsoft.AspNetCore.Components;

namespace Kelburg_frontend.Components.Pages.AdminPages;

public partial class Tickets : ComponentBase
{
    private List<Models.Tickets> TicketsList = new List<Models.Tickets>();

    private string TicketStatus = "All";
    private string Category = "All";
    private List<string> allCategories = new List<string>();
    
    private int pageSize = 10;
    private int pageNumber = 1;
    
    private bool isSearching = false;
    
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await AuthService.EnsureAdminAccess();

            isSearching = true;
            StateHasChanged();
            
            await GetTickets(pageSize, pageNumber);
            await GetCategories();
            StateHasChanged();
        }
    }

    private async Task GetTickets(int pageSize, int pageNumber)
    {
        TicketsList.Clear();
        
        Dictionary<string, object?> queryParams = new Dictionary<string, object?>()
        {
            {"pageSize", pageSize},
            {"pageNumber", pageNumber},
        };

        if (!Category.ToLower().Contains("all"))
        {
           queryParams.Add("ticketCategory", Category); 
        }
        
        if (!TicketStatus.ToLower().Contains("all"))
        {
            queryParams.Add("ticketStatus", TicketStatus);
        }

        try
        {
            TicketsList = await APIHandler.RequestAPI<List<Models.Tickets>>(eTables.Tickets.Read, queryParams, HttpMethod.Get);
        }
        catch (Exception e) 
        {
            string err = e.Message;
            Console.WriteLine(err);
        }
    }
    
    private async Task GetCategories()
    {
        foreach(Models.Tickets ticket in TicketsList)
        {
            if (!allCategories.Contains(ticket.Category))
            {
                allCategories.Add(ticket.Category);
            }
        }
    }
    
    private async Task SearchClicked()
    {
        
    }

    
    // private async Task NextPage()
    // {
    //     pageNumber++;
    //     await SearchUsers(pageSize, pageNumber);
    // }
    //
    // private async Task PreviousPage()
    // {
    //     pageNumber--;
    //     await SearchUsers(pageSize, pageNumber);
    // }
    
    private async Task ClickRedirect(Models.Tickets ticket)
    {
        NavigationManager.NavigateTo($"/ticket/{ticket.Id}");
    }
}