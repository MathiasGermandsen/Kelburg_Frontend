using Microsoft.AspNetCore.Components;
using Kelburg_frontend.Models;

namespace Kelburg_frontend.Components.Pages.UserPages;

public partial class Rooms : ComponentBase
{
   private IReadOnlyList<DateTime?> dateRange;
   private int occupantsNumber;
   private List<Models.Rooms> availableRooms = new List<Models.Rooms>();
   private int pageSize = 12;
   private int pageNumber = 1;

   protected override async Task OnInitializedAsync()
   {
      await SearchRooms(pageSize, 1);
      StateHasChanged();
   }

   private async Task ClickRedirect(Models.Rooms room)
   {
      RoomsService.SetSelectedRoom(room);
      
      if (true) // Should eventually be User.LoggedIn ALEXANDER MAKE IT
      {
         NavigationManager.NavigateTo("/createBooking");
      }
      else // Will then trigger if the user is NOT logged in
      {
         NavigationManager.NavigateTo("/login");
      }
   }

   private async Task NextPage()
   {
      pageNumber++;
      await SearchRooms(pageSize, pageNumber);
   }

   private async Task PreviousPage()
   {
      pageNumber--;
      await SearchRooms(pageSize, pageNumber);
   }

   private async Task SearchClicked()
   {
      await SearchRooms(pageSize, 1);
   }

   private async Task SearchRooms(int pageSize, int pageNumber)
   {
      string endpointToUse = eTables.Rooms.AvailableBetweenDates;

      Dictionary<string, object?> queryParams = new Dictionary<string, object?>()
      {
         {"pageSize", pageSize},
         {"pageNumber", pageNumber},
      };
      
      if (dateRange == null)
      {
         endpointToUse = eTables.Rooms.Read;
      }
      else
      {
         queryParams.Add("startDate", dateRange[0].Value.Date.ToString("yyyy-MM-dd"));
         queryParams.Add("endDate", dateRange[1].Value.Date.ToString("yyyy-MM-dd"));
      }
      
      if (occupantsNumber >= 1)
      {
         queryParams.Add("roomSize", occupantsNumber);
      }
      
      availableRooms = await APIHandler.RequestAPI<List<Models.Rooms>>(endpointToUse, queryParams, HttpMethod.Get);
      
      foreach (Models.Rooms room in availableRooms)
      {
         room.RoomImagePath = room.GetRandomImagePath();
      }
   }
}