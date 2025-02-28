using Microsoft.AspNetCore.Components;

namespace Kelburg_frontend.Components.Pages.AdminPages;

public partial class Booking : ComponentBase
{
    [Parameter]
    public string idParam { get; set; }
    public int BookingId { get; set; }
    
    private Models.Bookings selectedBooking = new Models.Bookings();
    private Models.Users bookingUser = new Models.Users();
    private Models.Rooms bookingRoom = new Models.Rooms();
    private Models.HotelCars bookingCar = null;
    private Models.Services bookingService = new Models.Services();
    private bool modalVisible = false;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await AuthService.EnsureAdminAccess();
            
            if (int.TryParse(idParam, out int bookingId))
            {
                BookingId = bookingId;
            }

            await GetBooking();
            await GetUser();
            await GetRoom();
            await GetCar();
            await GetService();
            
            StateHasChanged();
        }
    }

    private async Task GetBooking()
    {
        Dictionary<string, object?> queryParams = new Dictionary<string, object?>()
        {
            {"pageSize", 1},
            {"pageNumber", 1},
            {"bookingId", BookingId}
        };
        
        List<Models.Bookings> selectedBookingList = await APIHandler.RequestAPI<List<Models.Bookings>>(eTables.Bookings.Read, queryParams, HttpMethod.Get);
        selectedBooking = selectedBookingList.FirstOrDefault();
    }
    
    private async Task GetUser()
    {
        Dictionary<string, object?> queryParams = new Dictionary<string, object?>()
        {
            {"pageSize", 1},
            {"pageNumber", 1},
            {"userId", selectedBooking.UserId}
        };
        
        List<Models.Users> selectedUserList = await APIHandler.RequestAPI<List<Models.Users>>(eTables.Users.Read, queryParams, HttpMethod.Get, null, await AuthService.GetToken());
        bookingUser = selectedUserList.FirstOrDefault();
    }

    private async Task GetRoom()
    {
        Dictionary<string, object?> queryParams = new Dictionary<string, object?>()
        {
            {"pageSize", 1},
            {"pageNumber", 1},
            {"roomId", selectedBooking.RoomId}
        };
        
        List<Models.Rooms> selectedRoomList = await APIHandler.RequestAPI<List<Models.Rooms>>(eTables.Rooms.Read, queryParams, HttpMethod.Get);
        bookingRoom = selectedRoomList.FirstOrDefault();
        
        if (bookingRoom != null)
        {
            bookingRoom.RoomImagePath = bookingRoom.GetRandomImagePath();
        }
    }
    
    private async Task GetCar()
    {
        if (selectedBooking.CarId == null)
        {
            return;
        }
        
        Dictionary<string, object?> queryParams = new Dictionary<string, object?>()
        {
            {"pageSize", 1},
            {"pageNumber", 1},
            {"carId", selectedBooking.CarId}
        };
        
        List<Models.HotelCars>? selectedCarList = await APIHandler.RequestAPI<List<Models.HotelCars>>(eTables.HotelCars.Read, queryParams, HttpMethod.Get);
        bookingCar = selectedCarList.FirstOrDefault();
    }
    
    private async Task GetService()
    {
        Dictionary<string, object?> queryParams = new Dictionary<string, object?>()
        {
            {"serviceId", selectedBooking.ServiceId}
        };
        
        List<Models.Services> selectedServiceList = await APIHandler.RequestAPI<List<Models.Services>>(eTables.Services.Read, queryParams, HttpMethod.Get);
        bookingService = selectedServiceList.FirstOrDefault();
        
        bookingService.PrettyName = bookingService.AllInclusive 
            ? "All Inclusive" : bookingService.Breakfast 
            ? "Breakfast" : bookingService.Dinner 
            ? "Dinner" : bookingService.BreakfastAndDinner 
            ? "Breakfast and Dinner" : "No service";
    }
    
    private async Task EditBooking()
    {
        modalVisible = true;
        StateHasChanged();
    }

    private async Task SaveChanges()
    {
        
    }

    private async Task CloseModal()
    {
        modalVisible = false;
        StateHasChanged();
    }
}