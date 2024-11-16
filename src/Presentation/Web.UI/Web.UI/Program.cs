using System.Net.Http.Headers;
using System.Runtime.Versioning;

using Blazored.LocalStorage;

using Library.Validations;

using Microsoft.AspNetCore.Components.Authorization;

using Web.UI.Components;
using Web.UI.Middlewares;
using Web.UI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services
    .AddAuthorization()
    .AddAuthentication();

builder.Services.AddMemoryCache();

var baseAddress = builder.Configuration["ApiSettings:BaseAddress"];
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(baseAddress) });

builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(baseAddress);
});

builder.Services.AddBlazoredLocalStorage();
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

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    //.AddAdditionalAssemblies(typeof(Web.UI.Client._Imports).Assembly)
    ;

await app.RunAsync();