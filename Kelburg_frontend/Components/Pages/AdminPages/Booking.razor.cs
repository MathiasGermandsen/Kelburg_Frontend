using Microsoft.AspNetCore.Components;

namespace Kelburg_frontend.Components.Pages.AdminPages;

public partial class Booking : ComponentBase
{
    [Parameter]
    public string idParam { get; set; }
    public int BookingId { get; set; }
    
    private Models.Bookings selectedBooking = new Models.Bookings();
    private Models.Bookings editedBooking = new Models.Bookings();
    private Models.Users bookingUser = new Models.Users();
    private Models.Rooms bookingRoom = new Models.Rooms();
    
    private Models.HotelCars bookingCar = null;
    private List<Models.HotelCars> carList = new List<Models.HotelCars>();
    
    private Models.Services bookingService = new Models.Services();
    private List<Models.Services> serviceList = new List<Models.Services>();
    
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
        Dictionary<string, object?> queryParams = new Dictionary<string, object?>()
        {
            {"startDate", selectedBooking.StartDate.ToString("yyyy-MM-dd")},
            {"endDate", selectedBooking.StartDate.ToString("yyyy-MM-dd")},
            {"pageSize", 200},
            {"pageNumber", 1}
        };
        
        carList = await APIHandler.RequestAPI<List<Models.HotelCars>>(eTables.HotelCars.AvailableBetweenDates, queryParams, HttpMethod.Get);
        bookingCar = carList.FirstOrDefault(c => c.Id == selectedBooking.CarId);
    }
    
    private async Task GetService()
    {
        Dictionary<string, object?> queryParams = new Dictionary<string, object?>();
        
        serviceList = await APIHandler.RequestAPI<List<Models.Services>>(eTables.Services.Read, queryParams, HttpMethod.Get);
        bookingService = serviceList.FirstOrDefault(s => s.Id == selectedBooking.ServiceId);
        
        foreach (Models.Services service in serviceList)
        {
            service.PrettyName = service.AllInclusive 
                ? "All Inclusive" : service.Breakfast 
                ? "Breakfast" : service.Dinner 
                ? "Dinner" : service.BreakfastAndDinner 
                ? "Breakfast and Dinner" : "No service";
        }
    }
    
    private async Task EditBooking()
    {
        editedBooking = selectedBooking;
        modalVisible = true;
        StateHasChanged();
    }
    
    private void OnServiceChanged(ChangeEventArgs e)
    {
        editedBooking.ServiceId = Convert.ToInt32(e.Value);
    }
    
    private void OnCarChanged(ChangeEventArgs e)
    {
        if (Convert.ToInt32(e.Value) == 0)
        {
            editedBooking.CarId = null;
        }
        else
        {
            editedBooking.CarId = Convert.ToInt32(e.Value);
        }
    }

    private async Task SaveChanges()
    {
       Dictionary<string, object?> queryParams = new Dictionary<string, object?>()
       {
           {"bookingIdToChange", editedBooking.Id},
           {"ServiceId", editedBooking.ServiceId},
       };

       if (editedBooking.CarId != null)
       {
           queryParams.Add("CarId", editedBooking.CarId);
       }

       Models.Bookings updatedBooking = await APIHandler.RequestAPI<Models.Bookings>(eTables.Bookings.Update, queryParams, HttpMethod.Put);
       
       if (updatedBooking != null)
       {
           await GetBooking();
       }

       NavigationManager.NavigateTo(NavigationManager.Uri, true);
    }

    private async Task CloseModal()
    {
        modalVisible = false;
        StateHasChanged();
    }

}