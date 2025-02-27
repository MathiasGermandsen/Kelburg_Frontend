using System.Security.Principal;
using Microsoft.AspNetCore.Components;
using Kelburg_frontend.Models;
using Blazorise;

namespace Kelburg_frontend.Components.Pages.AdminPages;

public partial class AdminRoom : ComponentBase
{
    private List<Models.Rooms> allRooms = new();
    private List<Models.Rooms> filteredRooms = new List<Models.Rooms>();

    private IReadOnlyList<DateTime?> dateRange;
    private int occupantsNumber;
    private bool isLoading = true;
    private int pageSize = 12;
    private int pageNumber = 1;
    private string sortByBooked = "";
    private string sortByOccupants = "";

    protected override async Task OnInitializedAsync()
    {
        await LoadAllRooms(pageSize, pageNumber);
        filteredRooms = new List<Models.Rooms>(allRooms);
        isLoading = false;
    }

    private async Task LoadAllRooms(int pageSize, int pageNumber)
    {
        Dictionary<string, object?> queryParams = new Dictionary<string, object?>()
        {
            { "pageSize", pageSize },
            { "pageNumber", pageNumber },
        };

        allRooms = await APIHandler.RequestAPI<List<Models.Rooms>>(eTables.Rooms.Read, queryParams, HttpMethod.Get);
        filteredRooms = new List<Models.Rooms>(allRooms);

        foreach (Models.Rooms room in allRooms)
        {
            room.RoomImagePath = room.GetRandomImagePath();
        }
    }

    private void SortByBookedChanged(ChangeEventArgs e)
    {
        sortByBooked = e.Value.ToString();
        ApplySorting();
    }

    private void SortByOccupantsChanged(ChangeEventArgs e)
    {
        sortByOccupants = e.Value.ToString();
        ApplySorting();
    }


    private async void ApplySorting()
    {
        filteredRooms = new List<Models.Rooms>(allRooms);
        
        Dictionary<string, object?> queryParams = new Dictionary<string, object?>()
        {
            { "sortByBooked", sortByBooked },
            { "sortByOccupants", sortByOccupants },
        };

        List<Models.Rooms> rooms = await APIHandler.RequestAPI<List<Models.Rooms>>(eTables.Rooms.UnavailableBetweenDates, queryParams, HttpMethod.Get);
        

        filteredRooms = new List<Models.Rooms>(rooms); // Clone the list

        if (!string.IsNullOrEmpty(sortByBooked))
        {
            if (bool.TryParse(sortByBooked, out bool isBooked))
            {
                filteredRooms = filteredRooms.Where(room => room.isBooked == isBooked).ToList();
            }
        }

        if (!string.IsNullOrEmpty(sortByOccupants))
        {
            if (!string.IsNullOrEmpty(sortByOccupants))
            {
                filteredRooms = sortByOccupants.Equals("asc", StringComparison.OrdinalIgnoreCase)
                    ? filteredRooms.OrderBy(room => room.Size).ToList()
                    : filteredRooms.OrderByDescending(room => room.Size).ToList();
            }
        }

        StateHasChanged();
    }


    private async Task NextPage()
    {
        pageNumber++;
        await LoadAllRooms(pageSize, pageNumber);
    }

    private async Task PreviousPage()
    {
        pageNumber--;
        await LoadAllRooms(pageSize, pageNumber);
    }

    private async Task ClickRedirect(Models.Rooms room)
    {
        RoomsService.SetSelectedRoom(room);
        NavigationManager.NavigateTo($"/roomDetails/{room.Id}");
    }
}