using Microsoft.AspNetCore.Components;
using Kelburg_frontend.Models;

namespace Kelburg_frontend.Components.Pages.AdminPages;

public partial class AdminBooking : ComponentBase
{
    private List<Models.Bookings> availableBookings = new List<Models.Bookings>();
    private int pageSize = 12;
    private int pageNumber = 1;
    private bool isSearching = false;
    private int? bookingsID;
    private int? roomsID;
    private int? usersID;

    [Inject]
    private NavigationManager NavigationManager { get; set; }

    [Parameter]
    public EventCallback<int> OnEditBooking { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadInitialBookings();
    }

    private async Task LoadInitialBookings()
    {
        isSearching = true;
        try
        {
            await SearchBookings(pageSize, pageNumber);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading bookings: {ex.Message}");
        }
        isSearching = false;
    }

    private async Task NextPage()
    {
        pageNumber++;
        await SearchBookings(pageSize, pageNumber);
    }

    private async Task PreviousPage()
    {
        if (pageNumber > 1)
        {
            pageNumber--;
            await SearchBookings(pageSize, pageNumber);
        }
    }

    private async Task SearchBookings(int pageSize, int pageNumber)
    {
        
        Dictionary<string, object?> queryParams = new Dictionary<string, object?>()
        {
            {"pageSize", pageSize},
            {"pageNumber", pageNumber},
        };

        if (bookingsID != 0 && bookingsID != null)
        {
            queryParams.Add("bookingId", bookingsID);
        }
        if (usersID != 0 && usersID != null)
        {
            queryParams.Add("userId", usersID);
        }
        if (roomsID != 0 && roomsID != null)
        {
            queryParams.Add("roomId", roomsID);
        }
        
        try
        {
            var response = await APIHandler.RequestAPI<List<Models.Bookings>>(eTables.Bookings.Read, queryParams, HttpMethod.Get);
            availableBookings = response ?? new List<Models.Bookings>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching bookings: {ex.Message}");
            availableBookings = new List<Models.Bookings>();
        }
    }

    private async Task ClickRedirect(Models.Bookings booking)
    {
        if (booking == null)
        {
            Console.WriteLine("Invalid booking selected");
            return;
        }
        
        NavigationManager.NavigateTo($"/edit-booking/{booking.Id}");
    }
}