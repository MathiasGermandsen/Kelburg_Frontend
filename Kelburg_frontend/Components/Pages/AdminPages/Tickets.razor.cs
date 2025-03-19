using Microsoft.AspNetCore.Components;

namespace Kelburg_frontend.Components.Pages.AdminPages;

public partial class Tickets : ComponentBase
{
    private List<Models.Tickets> TicketsList = new List<Models.Tickets>();

    private string TicketStatus = "All";
    private List<string> allStatuses = new List<string>();
    
    private string TicketCategory = "All";
    private List<string> allCategories = new List<string>();

    private string sortBy = "desc"; 
    
    private int pageSize = 12;
    private int pageNumber = 1;
    
    private bool isSearching = false;
    
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await AuthService.EnsureAdminAccess();
            
            StateHasChanged();
            
            await SearchTickets(pageSize, pageNumber);
            await GetCategories();
            await GetStatuses();
            StateHasChanged();
        }
    }

    private async Task SearchTickets(int pageSize, int pageNumber)
    {
        isSearching = true;
        TicketsList.Clear();
        
        Dictionary<string, object?> queryParams = new Dictionary<string, object?>()
        {
            {"pageSize", pageSize},
            {"pageNumber", pageNumber},
        };

        if (!TicketCategory.ToLower().Contains("all"))
        {
           queryParams.Add("ticketCategory", TicketCategory); 
        }
        
        if (!TicketStatus.ToLower().Contains("all"))
        {
            queryParams.Add("ticketStatus", TicketStatus);
        }

        try
        {
            TicketsList = await APIHandler.RequestAPI<List<Models.Tickets>>(eTables.Tickets.Read, queryParams, HttpMethod.Get);
            
            if (sortBy == "asc")
            {
                TicketsList = TicketsList.OrderBy(c => c.CreatedAt).ToList();
            }
            else
            {
                TicketsList = TicketsList.OrderByDescending(c => c.CreatedAt).ToList();
            }
        }
        catch (Exception e) 
        {
            string err = e.Message;
            Console.WriteLine(err);
        }
        await AddDelay(150, 350);
        isSearching = false;
    }
    
    private async Task AddDelay(int min, int max)
    {
        Random random = new Random();
        Thread.Sleep(random.Next(min, max));
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
    
    private async Task GetStatuses()
    {
        foreach(Models.Tickets ticket in TicketsList)
        {
            if (!allStatuses.Contains(ticket.Status))
            {
                allStatuses.Add(ticket.Status);
            }
        }
    }
    
    private async Task SearchClicked()
    {
        await SearchTickets(pageSize, 1);
    }
    
    private async Task NextPage()
    {
        pageNumber++;
        await SearchTickets(pageSize, pageNumber);
    }
    
    private async Task PreviousPage()
    {
        pageNumber--;
        await SearchTickets(pageSize, pageNumber);
    }
    
    private async Task ClickRedirect(Models.Tickets ticket)
    {
        NavigationManager.NavigateTo($"/ticket/{ticket.Id}");
    }
}