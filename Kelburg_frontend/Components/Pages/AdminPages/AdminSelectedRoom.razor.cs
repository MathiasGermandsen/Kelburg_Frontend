using Microsoft.AspNetCore.Components;
using System.Security.Principal;
using Microsoft.AspNetCore.Components;
using Kelburg_frontend.Models;
using Blazorise;
using Kelburg_frontend.Services;

namespace Kelburg_frontend.Components.Pages.AdminPages;

public partial class AdminSelectedRoom : ComponentBase
{
    private List<string> nextAvailableDates = new();
    private string latestBookingDate = string.Empty;
    private string nextAvailableDate = string.Empty;
    
    private int occupantsNumber;

    private Models.Rooms selectedRoom;

    List<Models.Bookings> bookingsForRoom = new List<Models.Bookings>();

    [Inject] NavigationManager NavigationManager { get; set; }
    
    
    [Inject] private HttpClient Http { get; set; }

    [Parameter] public int RoomId { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await GetRoom();
            await GetBookingForRoom();

            if (bookingsForRoom.Any())
            {
                latestBookingDate = bookingsForRoom.First().EndDate.AddDays(1).ToString("dd/MM/yyyy");
            }
            
            nextAvailableDates.Add(latestBookingDate);
            
            StateHasChanged();
        }
    }

    private async Task GetRoom()
    {
        Dictionary<string, object?> queryParams = new Dictionary<string, object?>()
        {
            { "roomId", RoomId }
        };

        List<Models.Rooms> selectedRoomList = await APIHandler.RequestAPI<List<Models.Rooms>>(eTables.Rooms.Read, queryParams, HttpMethod.Get);
        selectedRoom = selectedRoomList.FirstOrDefault();
        selectedRoom.RoomImagePath = selectedRoom.GetRandomImagePath();
    }
    

    private async Task GetBookingForRoom()
    {
        Dictionary<string, object?> queryParams = new Dictionary<string, object?>()
        {
            { "roomId", RoomId }
        };
        
        bookingsForRoom = await APIHandler.RequestAPI<List<Models.Bookings>>(eTables.Bookings.Read, queryParams, HttpMethod.Get);
        
        bookingsForRoom = bookingsForRoom.OrderByDescending(b => b.EndDate).ToList();
    }

    void NavigateToBooking(int id)
    {
        NavigationManager.NavigateTo($"/Booking/{id}");
    }
    
}