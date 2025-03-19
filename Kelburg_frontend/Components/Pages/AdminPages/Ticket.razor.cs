using Microsoft.AspNetCore.Components;

namespace Kelburg_frontend.Components.Pages.AdminPages;

public partial class Ticket : ComponentBase
{
    [Parameter]
    public string idParam { get; set; }

    private int TicketId;

    private Models.Tickets? SelectedTicket = new Models.Tickets();
    private Models.Users? TicketUser = new Models.Users();
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await AuthService.EnsureAdminAccess();
            
            if (int.TryParse(idParam, out int ticketId))
            {
                TicketId = ticketId;
            }
            
            await GetTicket();
            await GetTicketUser();
            StateHasChanged();
        }
    }

    private async Task GetTicket()
    {
         Dictionary<string, object?> queryParams = new Dictionary<string, object?>()
         {
             {"pageSize", 1},
             {"pageNumber", 1},
             {"ticketId", TicketId}
         };
         
         List<Models.Tickets> foundTicket = await APIHandler.RequestAPI<List<Models.Tickets>>(eTables.Tickets.Read, queryParams, HttpMethod.Get);
         SelectedTicket = foundTicket[0];
    }

    private async Task GetTicketUser()
    {
        Dictionary<string, object?> queryParams = new Dictionary<string, object?>()
        { 
            {"pageSize", 1},
            {"pageNumber", 1},
            {"userId", SelectedTicket.FromUser}
        };
        
        List<Models.Users>? foundUser = await APIHandler.RequestAPI<List<Models.Users>>(eTables.Users.Read, queryParams, HttpMethod.Get, null, await AuthService.GetToken());
        TicketUser = foundUser[0];
    }
}