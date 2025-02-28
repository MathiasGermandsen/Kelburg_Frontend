
namespace Kelburg_frontend.Models;

public class Rooms : Common
{
    public int Size { get; set; }
    public string RoomType { get; set; }
    public string ViewType { get; set; }
    public int PricePrNight { get; set; }
    public bool isBooked { get; set; }
    public string RoomImagePath { get; set; }

    private static readonly List<string> RoomImages = new List<string>()
    {
        "/images/rooms/room1/room1.jpg",
        "/images/rooms/room2/room2.jpg",
        "/images/rooms/room3/room3.jpg",
        "/images/rooms/room4/room4.jpg",
        "/images/rooms/room5/room5.jpg",
        "/images/rooms/room6/room6.jpg",
        "/images/rooms/room7/room7.jpg",
        "/images/rooms/room8/room8.jpg",
        "/images/rooms/room9/room9.jpg",
    };

    public string GetRandomImagePath()
    {
        Random random = new Random();
        
        int index = random.Next(0, RoomImages.Count);
        string imagePath = RoomImages[index];
        return imagePath;
    }
}
