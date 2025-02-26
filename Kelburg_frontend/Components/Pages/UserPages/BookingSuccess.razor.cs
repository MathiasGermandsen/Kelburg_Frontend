using Microsoft.AspNetCore.Components;
using Kelburg_frontend.Models;

namespace Kelburg_frontend.Components.Pages.UserPages;

public partial class BookingSuccess : ComponentBase
{
    private Bookings bookingToCreate = new Bookings();
    private Models.Rooms roomUsed = new Models.Rooms();
    private HotelCars carUsed = new HotelCars();
    private Models.Services serviceUsed = new Models.Services();
    
    private bool _successfullyCreated = false;
    private bool _errorCreating = false;
    private bool _isInitialized = false;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        bookingToCreate = await BookingService.GetBooking();
        roomUsed = await GetRoom();
        carUsed = await GetCar();
        serviceUsed = await GetService();
        
        if (firstRender && !_isInitialized)
        {
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
        
        List<Models.Rooms>? room = await APIHandler.RequestAPI<List<Models.Rooms>>(eTables.Rooms.Read, queryParams, HttpMethod.Get);
        return room[0];
    }

    private async Task<HotelCars?> GetCar()
    {
        if (!bookingToCreate.CarId.HasValue)
        {
            return null;
        }
        Dictionary<string, object?> queryParams = new Dictionary<string, object?>()
        {
            {"carId", bookingToCreate.CarId}
        };
        
        List<HotelCars>? car = await APIHandler.RequestAPI<List<HotelCars>>(eTables.HotelCars.Read, queryParams, HttpMethod.Get);
        return car[0];
    }

    private async Task<Models.Services> GetService()
    {
        Dictionary<string, object?> queryParams = new Dictionary<string, object?>()
        {
            {"serviceId", bookingToCreate.ServiceId}
        };
        
        List<Models.Services>? service = await APIHandler.RequestAPI<List<Models.Services>>(eTables.Services.Read, queryParams, HttpMethod.Get);
        
        service[0].PrettyName = service[0].AllInclusive 
            ? "All Inclusive" : service[0].Breakfast 
            ? "Breakfast" : service[0].Dinner 
            ? "Dinner" : service[0].BreakfastAndDinner 
            ? "Breakfast and Dinner" : "No service";
        return service[0];
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
        };
        
        if (bookingToCreate.CarId.HasValue)
        {
            queryParams.Add("CarId", bookingToCreate.CarId);
        }
        
        Bookings? bookingCreated = await APIHandler.RequestAPI<Bookings>(eTables.Bookings.Create, queryParams, HttpMethod.Post);
        
        if (bookingCreated != null)
        {
            return true;
        }
        else
        {
            _errorCreating = true;
            return false;
        }
    }

    private async Task ClickRedirect()
    {
        await BookingService.SetNewBooking(new Bookings());
        NavigationManager.NavigateTo("/");
    }
}