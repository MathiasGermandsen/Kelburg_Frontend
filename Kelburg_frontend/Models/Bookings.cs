﻿namespace Kelburg_frontend.Models;

public class Bookings : Common
{
    public int UserId { get; set; }
    public int PeopleCount { get; set; }
    public int BookingPrice { get; set; }
    public int RoomId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int ServiceId { get; set; }
    public int? CarId { get; set; }
    
    
    public int CalculateBookingPrice(Bookings currentBooking, Rooms selectedRoom, HotelCars selectedCar, List<Services> services)
    {
        int bookingPrice = 0;
        int vacationDays = (currentBooking.EndDate - currentBooking.StartDate).Days;

        if (vacationDays == 0)
        {
            vacationDays = 1;
        }

        int serviceIndex = currentBooking.ServiceId - 1;
        Services selectedService = services[serviceIndex];
        
        int totalServicePrices = selectedService.PricePrPersonPrNight * currentBooking.PeopleCount * vacationDays;        
        int totalRoomPrice = selectedRoom.PricePrNight * vacationDays;
        int totalCarPrice = 0;

        if (selectedCar != null)
        {
            totalCarPrice = selectedCar.PricePrNight * vacationDays;
        }
        
        bookingPrice = totalServicePrices + totalRoomPrice + totalCarPrice;
        
        return bookingPrice;
    }

    public bool CheckBookingOverlap(Bookings booking1, Bookings booking2)
    {
        return booking1.StartDate < booking2.EndDate && booking2.StartDate < booking1.EndDate;
    }
}

public class BookingCreateDTO
{
    public int UserId { get; set; }
    public int PeopleCount { get; set; }
    public int RoomId { get; set; }
    public DateTime StartDate { get; set; } = DateTime.Today.Date;
    public DateTime EndDate { get; set; } = DateTime.Today.Date;
    public int ServiceId { get; set; }
    public int? CarId { get; set; } 
}

public class BookingEditDTO
{
    public int PeopleCount { get; set; }
    public int RoomId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int ServiceId { get; set; }
    public int? CarId { get; set; }
}

