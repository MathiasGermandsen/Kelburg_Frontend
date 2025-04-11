using Microsoft.AspNetCore.Components;

namespace Kelburg_frontend.Components.Pages.UserPages;

public partial class RoomComparison : ComponentBase
{
    private Models.Rooms room1 = new Models.Rooms();
    private Models.Rooms room2 = new Models.Rooms();
    private bool isLoading = false;
    
    Models.Bookings currentBooking = new Models.Bookings();
    
    [Parameter] public int room1Id { get; set; }
    [Parameter] public int room2Id { get; set; }

    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            currentBooking = await BookingService.GetBooking();
            
            isLoading = true;
            room1 = await GetRoomById(room1Id); 
            room2 = await GetRoomById(room2Id);
            isLoading = false;
            
            StateHasChanged();
        }
    }

    private async Task<Models.Rooms> GetRoomById(int roomId)
    {
        Dictionary<string, object?> queryParams = new Dictionary<string, object?>()
        {
            {"roomId", roomId},
            {"pageSize", 1},
            {"pageNumber", 1},
        };
        Models.Rooms room = (await APIHandler.RequestAPI<List<Models.Rooms>>(eTables.Rooms.Read, queryParams, HttpMethod.Get))?.FirstOrDefault();
        room.RoomImagePath = room.GetRandomImagePath();
        
        return room;
    }

    private async Task SelectRoomForBooking(Models.Rooms room)
    {
        currentBooking.RoomId = room.Id;
        currentBooking.PeopleCount = room.Size;
        
        await BookingService.SetNewBooking(currentBooking);
        await RoomService.SetSelectedRoom(room);
        
        NavigationManager.NavigateTo($"/createBooking");
    }
}