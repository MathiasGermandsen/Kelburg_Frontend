﻿using Microsoft.AspNetCore.Components;
using Kelburg_frontend.Models;
using Kelburg_frontend.Services;

namespace Kelburg_frontend.Components.Pages.UserPages;

public partial class Booking : ComponentBase
{
    Bookings currentBooking = new Bookings();
    Models.Rooms currentRoom = new Models.Rooms();
    List<Models.Services> availableServices = new List<Models.Services>();
    List<HotelCars> availableCars = new List<HotelCars>();

    private bool isCheckingOut = false;
    int amountOfDays;
    
    int selectedServiceId;
    Models.Services selectedService;
    
    int selectedCarId;
    HotelCars selectedCar;
    
    Users User = new Users();
    
    protected override async Task OnInitializedAsync()
    {
        await GetUserInfo();
        
        currentBooking = await BookingService.GetBooking();
        currentBooking.UserId = User.Id;
        
        currentRoom = RoomsService.GetSelectedRoom();
        amountOfDays = (currentBooking.EndDate - DateTime.Now).Days + 2; // +2 Because of startday and endday are not included without
        
        await PopulateLists();
        GetPrice();

        StateHasChanged();
    }

    private async Task GetUserInfo()
    {
        string? token = await JSRuntime.InvokeAsync<string>("localStorage.getItem", new object[] { "authToken" });
        
        Dictionary<string, object?> queryParams = new Dictionary<string, object?>()
        {
            { "jwtToken", token }
        };
        
        User = await APIHandler.RequestAPI<Users>(eTables.Users.GetUserFromToken, queryParams, HttpMethod.Get);
    }
    
    private void GetPrice()
    {
        currentBooking.BookingPrice = currentBooking.CalculateBookingPrice(currentBooking, currentRoom, selectedCar, availableServices);
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
        availableCars = availableCars.OrderBy(c => c.Size).ToList();
    }

    private void OnServiceChanged(ChangeEventArgs e)
    {
        selectedServiceId = Convert.ToInt32(e.Value);
        OnSelectionChanged();
    }

    private void OnCarChanged(ChangeEventArgs e)
    {
        selectedCarId = Convert.ToInt32(e.Value);
        OnSelectionChanged();
    }

    private void OnSelectionChanged()
    {
        if (selectedServiceId != 0)
        {
            selectedService = availableServices.Where(s => s.Id == selectedServiceId).FirstOrDefault();
            currentBooking.ServiceId = selectedService.Id;
        }
        else
        {
            selectedService = null;
            currentBooking.ServiceId = 0;
        }

        if (selectedCarId != 0)
        {
            selectedCar = availableCars.Where(c => c.Id == selectedCarId).FirstOrDefault();
            currentBooking.CarId = selectedCar.Id;
        }
        else
        {
            selectedCar = null;
            currentBooking.CarId = null;
        }

        GetPrice();
    }

    private async Task CheckoutOrder()
    {
        isCheckingOut = true;
        
        await BookingService.SetNewBooking(currentBooking);
        string checkoutUrl = await BookingService.GetCheckout();
        
        NavigationManager.NavigateTo(checkoutUrl);
    }
}