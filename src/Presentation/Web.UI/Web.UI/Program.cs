using Blazored.LocalStorage;

using Web.UI.Components;
using Web.UI.Middlewares;
using Web.UI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddMemoryCache();
builder.Services.AddBlazoredLocalStorage(); // Register Blazored LocalStorage

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication();

// Configure HttpClient
var baseAddress = builder.Configuration["ApiSettings:BaseAddress"];
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(baseAddress) });
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(baseAddress);
});

builder.Services.AddScoped<ApiClientService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseAntiforgery();
app.UseAuthenticationMiddleware();

// Map Razor components
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode();

await app.RunAsync();
