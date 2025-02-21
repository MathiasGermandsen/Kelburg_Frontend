using Microsoft.AspNetCore.Components;
using Kelburg_frontend.Models;

namespace Kelburg_frontend.Components.Pages.UserPages;

public partial class Booking : ComponentBase
{
    Bookings currentBooking = new Bookings();
    List<Models.Services> availableServices = new List<Models.Services>();
    List<HotelCars> availableCars = new List<HotelCars>();

    Models.Services selectedService = new Models.Services();
    HotelCars selectedCar = new HotelCars();
    
    Users placeholderUser = new Users()
    {
        FirstName = "John",
        LastName = "Backflip",
        Email = "JohnBackflip@gmail.com",
        PhoneNumber = "22 11 33 11"
    };
    
    protected override async Task OnInitializedAsync()
    {
        currentBooking = BookingService.GetBooking();
        await PopulateLists();
        StateHasChanged();
    }

    private async Task PopulateLists()
    {
        Dictionary<string, object?> queryParams = new Dictionary<string, object?>();
        
        availableServices = await APIHandler.RequestAPI<List<Models.Services>>(eTables.Services.Read, queryParams, HttpMethod.Get);

        foreach (Models.Services service in availableServices)
        {
            service.PrettyName = service.AllInclusive 
                ? "All Inclusive" : service.Breakfast 
                ? "Breakfast" : service.Dinner 
                ? "Dinner" : service.BreakfastAndDinner 
                ? "Breakfast and Dinner" : "No service";
        }
        
        queryParams.Add("startDate", currentBooking.StartDate.ToString("yyyy-MM-dd"));
        queryParams.Add("endDate", currentBooking.StartDate.ToString("yyyy-MM-dd"));
        queryParams.Add("pageSize", 200);
        queryParams.Add("pageNumber", 1);
        
        availableCars = await APIHandler.RequestAPI<List<HotelCars>>(eTables.HotelCars.AvailableBetweenDates, queryParams, HttpMethod.Get);
    }
}