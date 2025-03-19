using Microsoft.AspNetCore.Components;
using System.Security.Principal;
using Microsoft.AspNetCore.Components;
using Kelburg_frontend.Models;
using Blazorise;

namespace Kelburg_frontend.Components.Pages.AdminPages;

public partial class AdminSelectedRoom : ComponentBase
{
    private List<string> nextAvailableDates = new();
    private List<string> bookedDates = new();

    private Rooms room;
    
    [Inject] private HttpClient Http { get; set; }
    
    [Parameter]
    public int RoomId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            room = await Http.GetFromJsonAsync<Models.Rooms>($"/api/room/{RoomId}");
            
            nextAvailableDates = await Http.GetFromJsonAsync<List<string>>("/api/room/next-available");
            bookedDates = await Http.GetFromJsonAsync<List<string>>("/api/room/booked");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error fetching data: {ex.Message}");
        }
    }
}