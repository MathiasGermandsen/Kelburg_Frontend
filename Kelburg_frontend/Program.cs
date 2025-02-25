using Blazored.LocalStorage;
using Kelburg_frontend.Components;
using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Kelburg_frontend.Models;
using Kelburg_frontend.Services;
using Blazored.LocalStorage;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddBlazorBootstrap();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<RoomsService>();
builder.Services.AddScoped<BookingService>();

builder.Services.AddScoped<HttpClient>(sp =>
    new HttpClient { BaseAddress = new Uri("https://localhost:44306/swagger/v1/swagger.json") }); // Replace with your API URL

builder.Services
    .AddBlazorise( options =>
    {
        options.Immediate = true;
    } )
    .AddBootstrap5Providers()
    .AddFontAwesomeIcons();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();