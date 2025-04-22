using Kelburg_frontend.Services;
using Microsoft.AspNetCore.Components;

namespace Kelburg_frontend.Components.Pages.UserPages;

public partial class RoomComparison : ComponentBase
{
    private bool isLoading = false;
    
    private List<Models.Rooms> roomsToCompareList = new List<Models.Rooms>();
    
    Models.Bookings currentBooking = new Models.Bookings();
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            
            currentBooking = await BookingService.GetBooking();
            
            isLoading = true;
            await GetRoomsToCompare();
            isLoading = false;
            
            StateHasChanged();
        }
    }

    private async Task GetRoomsToCompare()
    {
        try
        {
            for (int i = 1; i <= 10; i++)
            {
                string name = $"roomComparison{i}";
                Models.Rooms room = await RoomsService.GetCompareRoomToLocalstorage(name);
                
                if (room != null)
                {
                    roomsToCompareList.Add(room);
                }
                else
                {
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            
        }
        await RoomsService.DeleteLocalStorageByName("roomComparison");
    }

    private async Task SelectRoomForBooking(Models.Rooms room)
    {
        currentBooking.RoomId = room.Id;
        currentBooking.PeopleCount = room.Size;
        
        await BookingService.SetNewBooking(currentBooking);
        await RoomsService.SetSelectedRoom(room);
        
        NavigationManager.NavigateTo($"/createBooking");
    }
}