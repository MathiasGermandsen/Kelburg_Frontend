using Microsoft.AspNetCore.Components;

namespace Kelburg_frontend.Components.Pages.AdminPages;

public partial class StaffLandingpage : ComponentBase
{
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await AuthService.VerifyAdmin();
            StateHasChanged();
        }
    }
}