using System.Security.Principal;
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
   bool isSearching = false;

   private Models.Rooms roomComparison1 = null;
   private Models.Rooms roomComparison2 = null;


   private async Task ClickRedirect(Models.Rooms room)
   {
      Bookings bookingStart = new Bookings()
      {
         StartDate = dateRange[0].Value.Date,
         EndDate = dateRange[1].Value.Date,
         PeopleCount = occupantsNumber,
         RoomId = room.Id
      };
      
      await RoomsService.SetSelectedRoom(room);
      await BookingService.SetNewBooking(bookingStart);
      
      string? token = await JSRuntime.InvokeAsync<string>("localStorage.getItem", new object[] { "authToken" });
      
      if (!string.IsNullOrEmpty(token))
      {
         NavigationManager.NavigateTo("/createBooking");
      }
      else
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
      if (dateRange == null || occupantsNumber == 0)
      {
         return;
      }
      
      
      availableRooms.Clear();
      isSearching = true;
      await SearchRooms(pageSize, 1);
      
      await AddDelay(350, 750);
      
      isSearching = false;
   }

   private async Task AddDelay(int min, int max)
   {
      Random random = new Random();
      Thread.Sleep(random.Next(min, max));
   }

   private async Task SearchRooms(int pageSize, int pageNumber)
   {
      Dictionary<string, object?> queryParams = new Dictionary<string, object?>()
      {
         {"pageSize", pageSize},
         {"pageNumber", pageNumber},
         {"startDate", dateRange[0].Value.ToString("yyyy-MM-dd")},
         {"endDate", dateRange[1].Value.ToString("yyyy-MM-dd")},
         {"roomSize", occupantsNumber}
      };
      
      availableRooms = await APIHandler.RequestAPI<List<Models.Rooms>>(eTables.Rooms.AvailableBetweenDates, queryParams, HttpMethod.Get);
      
      foreach (Models.Rooms room in availableRooms)
      {
         room.RoomImagePath = room.GetRandomImagePath();
      }
   }

   private async Task AddRoomToCompare(Models.Rooms room)
   {
      if (roomComparison1 == null)
      {
         roomComparison1 = room;
      }
      else
      {
         roomComparison2 = room;
      }
      
      string? token = await JSRuntime.InvokeAsync<string>("localStorage.getItem", new object[] { "authToken" });

      if (roomComparison1 != null && roomComparison2 != null && !string.IsNullOrEmpty(token))
      {
         Bookings bookingStart = new Bookings()
         {
            StartDate = dateRange[0].Value.Date,
            EndDate = dateRange[1].Value.Date,
            PeopleCount = occupantsNumber,
         };
         
         BookingService.SetNewBooking(bookingStart);
         NavigationManager.NavigateTo($"/roomcompare/{roomComparison1.Id}&{roomComparison2.Id}");
      }
      else if (roomComparison1 != null && roomComparison2 != null)
      {
         NavigationManager.NavigateTo("/login");
      }
      StateHasChanged();
   }
}
