using Microsoft.AspNetCore.Components;
using Kelburg_frontend.Models;

namespace Kelburg_frontend.Components.Pages.UserPages;

public partial class BookingSuccess : ComponentBase
{
    private Bookings bookingToCreate = new Bookings();
    private Models.Rooms roomUsed = new Models.Rooms();
    private bool _successfullyCreated = false;
    private bool _isInitialized = false;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !_isInitialized)
        {
            bookingToCreate = await BookingService.GetBooking();
            roomUsed = await GetRoom();
            _successfullyCreated = await CreateBooking();
            _isInitialized = true;
            StateHasChanged();
        }
    }

    private async Task<Models.Rooms> GetRoom()
    {
        Dictionary<string, object?> queryParams = new Dictionary<string, object?>()
        {
            { "roomId", bookingToCreate.RoomId }
        };
        
        List<Models.Rooms> room = await APIHandler.RequestAPI<List<Models.Rooms>>(eTables.Rooms.Read, queryParams, HttpMethod.Get);
        return room[0];
    }

    private async Task<bool> CreateBooking()
    {
        Dictionary<string, object?> queryParams = new Dictionary<string, object?>()
        {
            {"UserId", bookingToCreate.UserId },
            {"PeopleCount", bookingToCreate.PeopleCount },
            {"RoomId", bookingToCreate.RoomId },
            {"StartDate", bookingToCreate.StartDate.ToString("yyyy-MM-dd") },
            {"EndDate", bookingToCreate.EndDate.ToString("yyyy-MM-dd") },
            {"ServiceId", bookingToCreate.ServiceId}, 
            {"CarId", bookingToCreate.CarId}
        };
        
        Bookings? bookingCreated = await APIHandler.RequestAPI<Bookings>(eTables.Bookings.Create, queryParams, HttpMethod.Post);

        if (bookingCreated != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void ClickRedirect()
    {
        NavigationManager.NavigateTo("/");
    }
}