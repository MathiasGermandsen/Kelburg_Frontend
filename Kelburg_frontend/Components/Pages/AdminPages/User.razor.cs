﻿using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace Kelburg_frontend.Components.Pages.AdminPages;

public partial class User : ComponentBase
{
    [Parameter]
    public string idParam { get; set; }
    public int UserId { get; set; }
    
    Models.Users selectedUser = new Models.Users();
    List<Models.Bookings> bookingsByUser = new List<Models.Bookings>();
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await AuthService.EnsureAdminAccess();
            
            if (int.TryParse(idParam, out int userId))
            {
                UserId = userId;
            }
            
            await GetUser();
            await GetBookingsByUser();
            StateHasChanged();
        }
    }
    
    private async Task GetUser()
    {
        Dictionary<string, object?> queryParams = new Dictionary<string, object?>()
        {
            {"pageSize", 1},
            {"pageNumber", 1},
            {"userId", UserId}
        };
        
        List<Models.Users> selectedUserList = await APIHandler.RequestAPI<List<Models.Users>>(eTables.Users.Read, queryParams, HttpMethod.Get, null, await AuthService.GetToken());
        selectedUser = selectedUserList.FirstOrDefault();
    }

    private async Task GetBookingsByUser()
    {
        Dictionary<string, object?> queryParams = new Dictionary<string, object?>()
        {
            {"pageSize", 8},
            {"pageNumber", 1},
            {"userId", selectedUser.Id}
        };
        
        bookingsByUser = await APIHandler.RequestAPI<List<Models.Bookings>>(eTables.Bookings.Read, queryParams, HttpMethod.Get);
        if (bookingsByUser.Any())
        {
            bookingsByUser = bookingsByUser.OrderByDescending(b => b.Id).ToList();
        }
    }

    private async Task ClickRedirect(Models.Bookings booking)
    {
        NavigationManager.NavigateTo($"/booking/{booking.Id}");
    }
}