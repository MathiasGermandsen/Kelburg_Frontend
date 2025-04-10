using Microsoft.AspNetCore.Components;

namespace Kelburg_frontend.Components.Pages.UserPages;

public partial class PhotoLibrary : ComponentBase
{
    private bool showPhotoModal = false;
    private Photo selectedPhoto;
    public class Photo
    {
        public string Title { get; set; }
        public string Image { get; set; }
    }
    
    private List<Photo> allPhotos = new()
    {
        new Photo { Title = "Beachfront View", Image = "/images/photoLibrary/BeachfrontView.png" },
        new Photo { Title = "City View", Image = "/images/photoLibrary/CityView.png" },
        new Photo { Title = "Courtyard View", Image = "/images/photoLibrary/CourtyardView.png" },
        new Photo { Title = "Desert View", Image = "/images/photoLibrary/DesertView.png" },
        new Photo { Title = "Garden View", Image = "/images/photoLibrary/GardenView.png" },
        new Photo { Title = "Golf Course View", Image = "/images/photoLibrary/Golf-CourseView.png" },
        new Photo { Title = "Lake View", Image = "/images/photoLibrary/LakeView.png" },
        new Photo { Title = "Marina View", Image = "/images/photoLibrary/MarinaView.png" },
        new Photo { Title = "Mountain View", Image = "/images/photoLibrary/MountainView.png" },
        new Photo { Title = "Park View", Image = "/images/photoLibrary/ParkView.png" },
        new Photo { Title = "Pool View", Image = "/images/photoLibrary/PoolView.png" },
        new Photo { Title = "River View", Image = "/images/photoLibrary/RiverView.png" },
        new Photo { Title = "Sea View", Image = "/images/photoLibrary/SeaView.png" },
        new Photo { Title = "Skyline View", Image = "/images/photoLibrary/SkylineView.png" }
    };

    private async Task ShowPhotoModal(Photo photo)
    {
        selectedPhoto = photo;
        showPhotoModal = true;
    }

    private async Task HidePhotoModal()
    {
        showPhotoModal = false;
    }
}