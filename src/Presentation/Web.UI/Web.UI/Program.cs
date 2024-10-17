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

var baseAddress = builder.Configuration["ApiSettings:BaseAddress"].ArgumentNotNull();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(baseAddress!) });

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