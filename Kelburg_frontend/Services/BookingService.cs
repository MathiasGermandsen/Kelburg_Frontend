using Blazored.LocalStorage;
using Kelburg_frontend.Models;
namespace Kelburg_frontend.Services;

public class BookingService
{
    private readonly ILocalStorageService _localStorage;

    public BookingService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }
    
    public event Action OnChange;

    public async Task SetNewBooking(Models.Bookings booking)
    {
        await _localStorage.SetItemAsync("booking", booking);
        NotifyStateChanged();
    }

    public async Task<Models.Bookings> GetBooking()
    {
        Bookings returnBooking = await _localStorage.GetItemAsync<Bookings>("booking");
        return returnBooking;
    }

    public async Task<string> GetCheckout()
    {
        string sessionUrl = await APIHandler.GetCheckoutSession(eTables.Payment.Checkout, await GetBooking());
        return sessionUrl;
    }
    
    private void NotifyStateChanged() => OnChange?.Invoke();
}