using System.Collections.Specialized;
using System.Net.Http.Headers;
using System.Text.Json;
using Blazored.SessionStorage;
using Kelburg_frontend.Models;

namespace Kelburg_frontend.Services;

public class LoginService
{
    //Storres and indicates different data
    private Users _currentUser;
    private bool _isLoggedIn;
    private string _authToken;
    private readonly ISessionStorageService _sessionStorage; //to interact with local storage
    private readonly HttpClient _httpClient; //For making http request with auth header
    
    public event Action OnChange; // To notify when the login state has changed

    public LoginService(ISessionStorageService sessionStorage, HttpClient httpClient)
    {
        _sessionStorage = sessionStorage; //asign the local storage service
        _httpClient = httpClient; // assign the http
    }
    
    //method to login the user
    public async Task Login(Users user, string token = null)
    {
        _currentUser = user; //set
        _isLoggedIn = true; //Mark user to logged in
        _authToken = token; // set auth token
        
        
        //serialize user object and save to LS
        //save to ILocalStorage (Migth try to save it in session)
        var userJson = JsonSerializer.Serialize(user);
        await _sessionStorage.SetItemAsync("currentuser", userJson);

        
        //Save auth token to LS
        if (!string.IsNullOrEmpty(token))
        {
            await _sessionStorage.SetItemAsync("authToken", token);
        }
        
        //make auth header for api request
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        NotifyChanged(); // notify user login state
    }

    //logout method
    public async Task LogoutAsync()
    {
        //to clear user, status and token
        _currentUser = null;
        _isLoggedIn = false;
        _authToken = null;

        // Clear user and token from local storage
        await _sessionStorage.RemoveItemAsync("currentuser");
        await _sessionStorage.RemoveItemAsync("authToken");

        // Remove the Authorization header in HttpClient
        _httpClient.DefaultRequestHeaders.Authorization = null;

        NotifyChanged();
    }

    public bool IsLoggedIn()
    {
        return _isLoggedIn;
    }
    
    public Users GetCurrentUser()
    {
        return _currentUser;
    }

    public string GetAuthToken()
    {
        return _authToken;
    }
    
    // method to initialize login, by cheking stored user and token in LS
    public async Task InitializeAsync()
    {
        //get token from LS
        _authToken = await _sessionStorage.GetItemAsync<string>("authToken");

        // If token exists, set for Authorization header
        if (!string.IsNullOrEmpty(_authToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
        }
        
        // to get serialized user obj from LS
        var userJson = await _sessionStorage.GetItemAsync<string>("currentuser");
        
        //if user data exist deserialize it and set current user and state
        if (!string.IsNullOrEmpty(userJson))
        {
            _currentUser = JsonSerializer.Deserialize<Users>(userJson);
            _isLoggedIn = true;
        }
    }
    
    
    private void NotifyChanged() => OnChange?.Invoke(); // to show login state changed
    
}