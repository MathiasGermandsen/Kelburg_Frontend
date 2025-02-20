using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Text;
using Kelburg_frontend.Models;

namespace Kelburg_frontend;

public class BookingsAPI : APIData
{
   public BookingsAPI(string urlPath) : base("Users", urlPath) {}

   public string Update => $"{_urlPath}/update";
}

public class RoomsAPI : APIData
{
    public RoomsAPI(string urlPath) : base("Rooms", urlPath) {}
    
    public string ChangePrice => $"{_urlPath}/changePrice";
    public string AvailableBetweenDates => $"{_urlPath}/availableBetweenDates";
}

public class ServicesAPI : APIData
{
    public ServicesAPI(string urlPath) : base("Services", urlPath) {}
    
    public string createPredefinedServices => $"{_urlPath}/createPredefinedServices";
    public string ChangePrice => $"{_urlPath}/changePrice";
}

public class UsersAPI : APIData
{
    public UsersAPI(string urlPath) : base("Users", urlPath) {}
    
    public string Login => $"{_urlPath}/login";
}

public static class eTables
{
    private const string APIBaseUrl = "https://localhost:44306/api";

    public static readonly BookingsAPI Bookings = new($"{APIBaseUrl}/Bookings");
    public static readonly APIData HotelCars = new ("HotelCars", $"{APIBaseUrl}/HotelCars");
    public static readonly RoomsAPI Rooms = new($"{APIBaseUrl}/Rooms"); 
    public static readonly ServicesAPI Services = new($"{APIBaseUrl}/Services");
    public static readonly APIData Tickets = new("Tickets", $"{APIBaseUrl}/Tickets");
    public static readonly UsersAPI Users = new($"{APIBaseUrl}/Users");
}

public static class APIHandler
{
    private static readonly HttpClient _httpClient = new HttpClient();
    public static async Task<T?> RequestAPI<T>(string apiUrl, Dictionary<string, object?> queryParams, HttpMethod method, object? body = null)
    {
        string queryParamString = string.Join("&", queryParams.Select(kvp => $"{HttpUtility.UrlEncode(kvp.Key)}={HttpUtility.UrlEncode(kvp.Value.ToString())}"));
        string completeApiUrl = $"{apiUrl}?{queryParamString}";
        Uri uri = new(completeApiUrl);
        Console.WriteLine(uri);
        try
        {
            using HttpRequestMessage request = new HttpRequestMessage(method, uri.AbsoluteUri);
            HttpResponseMessage response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            
            return JsonConvert.DeserializeObject<T>(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"API Request Failed - {ex.Message}");
            return default;
        }
    }
}