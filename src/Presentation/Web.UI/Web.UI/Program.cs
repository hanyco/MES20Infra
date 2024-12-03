using Blazored.LocalStorage;

using Microsoft.IdentityModel.Tokens;

using Web.UI.Components;
using Web.UI.Middlewares;
using Web.UI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add Razor Components
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(); // Or use .AddInteractiveWebAssemblyComponents()

// Add Authentication and Authorization
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "https://your-identity-provider";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false // Disable audience validation
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiAccess", policy =>
    {
        _ = policy.RequireAuthenticatedUser(); // Ensure authenticated users for API access
    });
});

// Configure HttpClient
var baseAddress = builder.Configuration["ApiSettings:BaseAddress"];
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(baseAddress); // Set base address for HTTP client
});

// Add other services
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<ApiClientService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging(); // Enable debugging in development mode
}
else
{
    _ = app.UseExceptionHandler("/Error"); // Use global error handler in production
}

app.UseStaticFiles();
app.UseAntiforgery();
app.UseAuthentication(); // Enable Authentication
app.UseAuthorization(); // Enable Authorization

// Add custom Authentication Middleware
app.UseMiddleware<AuthenticationMiddleware>();

// Map Razor Components
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode(); // Use server-side rendering mode

// Run the application
await app.RunAsync();
