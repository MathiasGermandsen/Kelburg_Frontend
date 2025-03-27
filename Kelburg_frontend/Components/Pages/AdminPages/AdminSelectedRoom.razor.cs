using Microsoft.AspNetCore.Components;
using System.Security.Principal;
using Microsoft.AspNetCore.Components;
using Kelburg_frontend.Models;
using Blazorise;
using Kelburg_frontend.Services;

namespace Kelburg_frontend.Components.Pages.AdminPages;

public partial class AdminSelectedRoom : ComponentBase
{
    private IReadOnlyList<DateTime?> dateRange;
    private List<string> nextAvailableDates = new();
    private List<string> bookedDates = new();
    
    private Rooms room = new();
    
    
    [Inject] private HttpClient Http { get; set; }
    
    [Parameter]
    public int RoomId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Dictionary<string, object?> queryParams = new Dictionary<string, object?>()
        {
            {"startDate", dateRange[0].Value.ToString("yyyy-MM-dd")},
            {"endDate", dateRange[1].Value.ToString("yyyy-MM-dd")},
        };
        
        try
        {
            room = await Http.GetFromJsonAsync<Models.Rooms>($"/api/room/{RoomId}");
            
            List<Models.Rooms> fetchedAvailableRooms = await APIHandler.RequestAPI<List<Models.Rooms>>(eTables.Rooms.AvailableBetweenDates, queryParams, HttpMethod.Get);
      
            List<Models.Rooms> bookedRooms = await APIHandler.RequestAPI<List<Models.Rooms>>(eTables.Rooms.UnavailableBetweenDates, queryParams, HttpMethod.Get);
            
            foreach (Rooms? room in bookedRooms)
            {
                room.isBooked = true;
            }
            
            nextAvailableDates = await Http.GetFromJsonAsync<List<string>>("/api/room/next-available");
            bookedDates = await Http.GetFromJsonAsync<List<string>>("/api/room/booked");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error fetching data: {ex.Message}");
        }
    }
}