using System.Security.Principal;
using Microsoft.AspNetCore.Components;
using Kelburg_frontend.Models;


namespace Kelburg_frontend.Components.Pages.AdminPages;

public partial class AdminRoom : ComponentBase
{
    private List<Models.Rooms> allRooms = new();
    private int occupantsNumber;
    private bool isLoading = true;
    private int pageSize = 12;
    private int pageNumber = 1;

    protected override async Task OnInitializedAsync()
    {
        await LoadAllRooms(pageSize, pageNumber);
        isLoading = false;
    }

    private async Task LoadAllRooms(int pageSize, int pageNumber)
    {
        Dictionary<string, object?> queryParams = new Dictionary<string, object?>()
        {
            {"pageSize", pageSize},
            {"pageNumber", pageNumber},
            // {"roomSize", occupantsNumber}
        };
        
        allRooms = await APIHandler.RequestAPI<List<Models.Rooms>>(eTables.Rooms.Read, queryParams, HttpMethod.Get);

        foreach (Models.Rooms room in allRooms)
        {
            room.RoomImagePath = room.GetRandomImagePath();
        }
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