using Microsoft.AspNetCore.Components;
using System.Security.Principal;
using Microsoft.AspNetCore.Components;
using Kelburg_frontend.Models;
using Blazorise;

namespace Kelburg_frontend.Components.Pages.AdminPages;

public partial class AdminSelectedRoom : ComponentBase
{
    private List<string> nextAvailableDates = new();
    private List<string> bookedDates = new();

    [Inject] private HttpClient Http { get; set; }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            nextAvailableDates = await Http.GetFromJsonAsync<List<string>>("/api/room/next-available");
            bookedDates = await Http.GetFromJsonAsync<List<string>>("/api/room/booked");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error fetching data: {ex.Message}");
        }
    }

    private void GoBack()
    {
        // Navigation logic (e.g., NavigationManager.NavigateTo("/previous-page"));
    }
}