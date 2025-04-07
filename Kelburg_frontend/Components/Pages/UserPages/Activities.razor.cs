using Microsoft.AspNetCore.Components;

namespace Kelburg_frontend.Components.Pages.UserPages;

public partial class Activities : ComponentBase
{
    
    class Activity
    {
        public string Title { get; set; }
        public string Image { get; set; }
    }
    
    private List<Activity> activities = new()
    {
        new Activity { Title = "Tour de Kjellerup", Image =  "/images/activities/tour.png" },
        new Activity { Title = "Water Gymnastics", Image = "/images/activities/water.png" },
        new Activity { Title = "Morning Hike", Image =  "/images/activities/downhike.png" },
        new Activity { Title = "Desert Camel Ride", Image = "/images/activities/camel.png" },
        new Activity { Title = "Forest Horse Ride", Image = "/images/activities/horse.png" },
        new Activity { Title = "Romantic Dinner", Image = "/images/activities/dinner.png" }
    };
    
    bool showPopup = false;
    string? selectedActivity = null;

    void ShowPopup(string title)
    {
        selectedActivity = title;
        showPopup = true;
    }

    void ClosePopup()
    {
        showPopup = false;
        selectedActivity = null;
    }
}
