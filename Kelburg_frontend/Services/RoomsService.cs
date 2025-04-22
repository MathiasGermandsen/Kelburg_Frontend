using Blazored.LocalStorage;

namespace Kelburg_frontend.Services;

public class RoomsService
{
    private readonly ILocalStorageService _localStorage;
    private Models.Rooms SelectedRoom { get; set; }

    public RoomsService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public event Action OnChange;

    public async Task SetSelectedRoom(Models.Rooms room)
    {
        SelectedRoom = room;
        NotifyStateChanged();
    }

    public async Task <Models.Rooms> GetSelectedRoom()
    {
        return SelectedRoom;
    }
    
    private void NotifyStateChanged() => OnChange?.Invoke();

    public async Task SetCompareRoomToLocalstorage(string name, Models.Rooms room)
    {
        await _localStorage.SetItemAsync(name, room);
        NotifyStateChanged();
    }

    public async Task<Models.Rooms> GetCompareRoomToLocalstorage(string name)
    {
        return await _localStorage.GetItemAsync<Models.Rooms>(name);
    }

    public async Task DeleteLocalStorageByName(string namesToDelete)
    {
        List<string> keys = (await _localStorage.KeysAsync()).ToList();

        foreach (string key in keys)
        {
            if (key.Contains(namesToDelete, StringComparison.OrdinalIgnoreCase))
            {
                await _localStorage.RemoveItemAsync(key);
            }
        }
    }
}