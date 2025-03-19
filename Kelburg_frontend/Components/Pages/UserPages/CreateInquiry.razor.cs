using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Components;

namespace Kelburg_frontend.Components.Pages.UserPages;

public partial class CreateInquiry : ComponentBase
{
    private List<string> Categories = new List<string>()
        { "Housekeeping", "Maintenance", "IT Support", "Room Service", "Support Services" };
    
    private string SelectedCategory = "";
    private string Description = "";
    
    private bool fieldEmpty = false;
    private bool errorCreating = false;
    
    private bool TicketCreated = false;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        string? token = await AuthService.GetToken();
        
        if (string.IsNullOrEmpty(token))
        {
            NavigationManager.NavigateTo("/login");
        }
        
        if (firstRender)
        {
            
        }
    }

    private async Task SendInquiry()
    {
        fieldEmpty = false;
        
        if (string.IsNullOrEmpty(Description) || string.IsNullOrEmpty(SelectedCategory))
        {
            fieldEmpty = true;
            StateHasChanged();
            return;
        }
        
        Dictionary<string, object?> queryParams = new Dictionary<string, object?>()
        {
            {"FromUser",  AuthService.GetUser().Id},
            {"Description", Description},
            {"Category", SelectedCategory},
            {"Status", "Open"}
        };

        Models.Tickets newTicket = await APIHandler.RequestAPI<Models.Tickets>(eTables.Tickets.Create, queryParams, HttpMethod.Post);
        
        if (newTicket == null)
        {
            errorCreating = true;
        }
        else
        {
            TicketCreated = true;
        }
    }
    
    private void CloseModal()
    {
        TicketCreated = false;
        NavigationManager.NavigateTo("/");
    }
}