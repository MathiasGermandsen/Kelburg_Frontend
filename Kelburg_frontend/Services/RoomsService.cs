namespace Kelburg_frontend.Services;

public class RoomsService
{
    private Models.Rooms SelectedRoom { get; set; }

    public event Action OnChange;

    public void SetSelectedRoom(Models.Rooms room)
    {
        SelectedRoom = room;
        NotifyStateChanged();
    }

    public Models.Rooms GetSelectedRoom()
    {
        return SelectedRoom;
    }
    
    private void NotifyStateChanged() => OnChange?.Invoke();
}