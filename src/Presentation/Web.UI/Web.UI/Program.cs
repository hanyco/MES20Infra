using System.Runtime.Versioning;


using Library.Validations;

using Microsoft.AspNetCore.Components.Authorization;

using Web.UI.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services
    .AddAuthorization()
    .AddAuthentication();

builder.Services.AddMemoryCache();

//var baseAddress = builder.Configuration["ApiSettings:BaseAddress"];

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(baseAddress) });

//builder.Services.AddHttpClient("MES.Web.UI", client =>
//{
//    client.BaseAddress = new Uri(baseAddress);
//}).AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

builder.Services.AddHttpClient("MES.Web.UI", client =>
{
    var baseAddress = builder.Configuration["ApiSettings:BaseAddress"];
    if (string.IsNullOrEmpty(baseAddress))
    {
        throw new InvalidOperationException("BaseAddress is not configured.");
    }

    client.BaseAddress = new Uri(baseAddress.TrimEnd('/'));
}).AddHttpMessageHandler<CustomAuthorizationMessageHandler>();



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

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Web.UI.Client._Imports).Assembly);

await app.RunAsync();