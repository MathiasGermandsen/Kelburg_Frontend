using Microsoft.AspNetCore.Components;

namespace Kelburg_frontend.Components.Pages.AdminPages;

public partial class User : ComponentBase
{
    [Parameter]
    public string idParam { get; set; }
    public int UserId { get; set; }
    
    protected override void OnParametersSet()
    {
        if (int.TryParse(idParam, out int userId))
        {
            UserId = userId;
        }
    }
}