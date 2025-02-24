namespace Kelburg_frontend.Services;

public class BookingService
{
    private Models.Bookings newBooking { get; set; }

    public event Action OnChange;

    public void SetNewBooking(Models.Bookings booking)
    {
        newBooking = booking;
        NotifyStateChanged();
    }

    public Models.Bookings GetBooking()
    {
        return newBooking;
    }
    
    private void NotifyStateChanged() => OnChange?.Invoke();
}