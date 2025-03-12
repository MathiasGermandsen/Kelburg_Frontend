using System.Security.Principal;
using Microsoft.AspNetCore.Components;
using Kelburg_frontend.Models;
using Blazorise;

namespace Kelburg_frontend.Components.Pages.AdminPages;

public partial class AdminRoom : ComponentBase
{
    private IReadOnlyList<DateTime?> dateRange;
   private int occupantsNumber;
   private List<Models.Rooms> availableRooms = new List<Models.Rooms>();
   private int pageSize = 12;
   private int pageNumber = 1;
   bool isSearching = false;
   private string sortOrder = "asc";
   private bool filterBooked = false;
   private string filterOption = "all";

   private async Task ClickRedirect(Models.Rooms room)
   {
      Models.Bookings bookingStart = new Models.Bookings()
      {
         StartDate = dateRange[0].Value.Date,
         EndDate = dateRange[1].Value.Date,
         PeopleCount = occupantsNumber,
         RoomId = room.Id
      };
      
      RoomsService.SetSelectedRoom(room);
      BookingService.SetNewBooking(bookingStart);
      
      string? token = await JSRuntime.InvokeAsync<string>("localStorage.getItem", new object[] { "authToken" });
      
      if (!string.IsNullOrEmpty(token)) 
      {
         NavigationManager.NavigateTo($"/selectedRoom/{room.Id}");
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
      if (dateRange == null)
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
      };

      if (occupantsNumber > 0)
      {
         queryParams.Add("Size", occupantsNumber);
      }
      
      List<Models.Rooms> fetchedAvailableRooms = await APIHandler.RequestAPI<List<Models.Rooms>>(eTables.Rooms.AvailableBetweenDates, queryParams, HttpMethod.Get);
      
      List<Models.Rooms> bookedRooms = await APIHandler.RequestAPI<List<Models.Rooms>>(eTables.Rooms.UnavailableBetweenDates, queryParams, HttpMethod.Get);
      foreach (Rooms? room in bookedRooms)
      {
         room.isBooked = true;
      }
      
      //combine lists
      availableRooms = fetchedAvailableRooms.Concat(bookedRooms).ToList();
      
      if (filterOption == "available")
      {
         availableRooms = availableRooms.Where(r => !r.isBooked).ToList();
      }
      else if (filterOption == "booked")
      {
         availableRooms = availableRooms.Where(r => r.isBooked).ToList();
      }
      
      availableRooms = sortOrder == "asc"
         ? availableRooms.OrderBy(r => r.Size).ToList()
         : availableRooms.OrderByDescending(r => r.Size).ToList();
      
      foreach (Models.Rooms room in availableRooms)
      {
         room.RoomImagePath = room.GetRandomImagePath();
      }
   }
}