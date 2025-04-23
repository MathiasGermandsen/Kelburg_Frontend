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
    public string UnavailableBetweenDates => $"{_urlPath}/unavailableBetweenDates";
    
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
    public string GetUserFromToken => $"{_urlPath}/getUserFromToken";
}

public class CarsAPI : APIData
{
    public CarsAPI(string urlPath) : base("HotelCars", urlPath) {}
    
    public string AvailableBetweenDates => $"{_urlPath}/availableBetweenDates";
}

public class PaymentAPI : APIData
{
    public PaymentAPI(string urlPath) : base("Payment", urlPath) {}
    
    public string Checkout => $"{_urlPath}/checkout";
}

public class TicketAPI : APIData
{
    public TicketAPI(string urlPath) : base("Payment", urlPath) {}
    
    public string UpdateStatus => $"{_urlPath}/updateStatus";
}

public static class eTables
{
    private const string APIBaseUrl = "http://10.133.51.102:8080/api";

    public static readonly BookingsAPI Bookings = new($"{APIBaseUrl}/Bookings");
    public static readonly CarsAPI HotelCars = new($"{APIBaseUrl}/HotelCars");
    public static readonly RoomsAPI Rooms = new($"{APIBaseUrl}/Rooms"); 
    public static readonly ServicesAPI Services = new($"{APIBaseUrl}/Services");
    public static readonly TicketAPI Tickets = new($"{APIBaseUrl}/Tickets");
    public static readonly UsersAPI Users = new($"{APIBaseUrl}/Users");
    public static readonly PaymentAPI Payment = new($"{APIBaseUrl}/Payment");
}

public static class APIHandler
{
    private static readonly HttpClient _httpClient = new HttpClient(
        new HttpClientHandler { ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true }
    );    
    public static async Task<T?> RequestAPI<T>(string apiUrl, Dictionary<string, object?> queryParams, HttpMethod method, object? body = null, string? token = null)
    {
        string queryParamString = string.Empty;
        string completeApiUrl = apiUrl;
            
        if (queryParams != null)
        {
            queryParamString = string.Join("&", queryParams.Select(kvp => $"{HttpUtility.UrlEncode(kvp.Key)}={HttpUtility.UrlEncode(kvp.Value.ToString())}"));
            completeApiUrl = $"{apiUrl}?{queryParamString}";
        }
        
        Uri uri = new Uri(completeApiUrl);
        string json = string.Empty;
      
        try
        {
            using HttpRequestMessage request = new HttpRequestMessage(method, uri.AbsoluteUri);
            
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            
            if (body != null)
            {   
                request.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            }
            
            HttpResponseMessage response = await _httpClient.SendAsync(request);

            json = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            
            if (typeof(T) == typeof(string))
            {
                return (T)(object)json;
            }
            
            return JsonConvert.DeserializeObject<T>(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"API Request Failed - {ex.Message}");
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine(json);
            Console.WriteLine("----------------------------------------------------------");
            
            return default;
        }
    }

    public static async Task<string> GetCheckoutSession(string apiUrl, Bookings booking)
    {
        string completeApiUrl = $"{apiUrl}";
        Uri uri = new Uri(completeApiUrl);
        
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(uri.AbsoluteUri, booking);
        string url = await response.Content.ReadAsStringAsync();
        
        return url;
    }
}