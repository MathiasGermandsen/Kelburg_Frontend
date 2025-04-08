using System.Security.Principal;
using Microsoft.AspNetCore.Components;
using Kelburg_frontend.Models;
using Blazorise;

namespace Kelburg_frontend.Components.Pages.AdminPages;

public partial class AdminRoom : ComponentBase
{
   private bool showCreateRoomModal = false;
   private Models.Rooms NewRoom = new Models.Rooms();
   
   private bool createdRoomFailed = false;
   private string createdRoomErrorMessage = "";
   
   private Dictionary<int, (string RoomType, int PricePrNight)> RoomTypeMapping = new()
   {
      { 1, ("Single room", 700) },
      { 2, ("Double room", 1260) },
      { 3, ("Triple room", 1400) },
      { 4, ("Quad room", 1750) },
      { 5, ("Suite", 2100) },
      { 6, ("Deluxe Room", 1960) },
      { 7, ("Presidential Suites", 3500) },
      { 8, ("Presidential Suites", 3500) },
      { 9, ("Presidential Suites", 3500) },
      { 10, ("Presidential Suites", 3500) },
      { 11, ("Presidential Suites", 3500) },
      { 12, ("Presidential Suites", 3500) }
   };

   private Dictionary<string, int> RoomViewTypeMapping = new Dictionary<string, int>()
   {
      { "Ocean view", 350 },
      { "Sea view", 315 },
      { "Mountain view", 210 },
      { "City view", 175 },
      { "Garden view", 140 },
      { "Pool view", 245 },
      { "Lake view", 280 },
      { "River view", 210 },
      { "Beachfront view", 385 },
      { "Park view", 140 },
      { "Skyline view", 280 },
      { "Courtyard view", 105 },
      { "Marina view", 350 },
      { "Forest view", 175 },
      { "Golf course view", 245 },
      { "Desert view", 105 }
   };
   
   private IReadOnlyList<DateTime?> dateRange;
   private int occupantsNumber;
   private List<Models.Rooms> availableRooms = new List<Models.Rooms>();
   private int pageSize = 12;
   private int pageNumber = 1;
   bool isSearching = false;
   private string sortOrder = "asc";
   private bool filterBooked = false;
   private string filterOption = "all";

   private async Task ShowCreateRoomModal()
   {
      showCreateRoomModal = true;
   }

   private async Task CloseCreateRoomModal()
   {
      showCreateRoomModal = false;
   }

   private async Task AddNewRoom()
   {
      createdRoomFailed = false;
      createdRoomErrorMessage = "";
      
      try
      {
         Dictionary<string, object?> queryParams = new Dictionary<string, object?>()
         {
            { "Size", NewRoom.Size },
            { "RoomType", RoomTypeMapping[NewRoom.Size].RoomType },
            { "ViewType", NewRoom.ViewType },
            { "PricePrNight", RoomTypeMapping[NewRoom.Size].PricePrNight + RoomViewTypeMapping[NewRoom.ViewType] }
         };
         Models.Rooms createdRoom = await APIHandler.RequestAPI<Models.Rooms>(eTables.Rooms.Create, queryParams, HttpMethod.Post);
         
         NavigationManager.NavigateTo(NavigationManager.Uri, forceLoad: true);
         
      }
      catch (Exception ex)
      {
         createdRoomFailed = true;
         
         if (ex.Message.ToLower().Contains("dictionary"))
         {
            createdRoomErrorMessage = "Size must be >0 and <12";
         }
         else if (ex.Message.ToLower().Contains("parameter"))
         {
            createdRoomErrorMessage = "Select View-Type";
         }
         else
         {
            createdRoomErrorMessage = ex.Message;

         }
      }
    
   }
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
      await BookingService.SetNewBooking(bookingStart);
      
      string? token = await JSRuntime.InvokeAsync<string>("localStorage.getItem", new object[] { "authToken" });
      
      if (!string.IsNullOrEmpty(token)) 
      {
         NavigationManager.NavigateTo($"/AdminSelectedRoom/{room.Id}");
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