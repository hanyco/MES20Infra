using HanyCo.Infra.Security.Client.Providers;

using Library.Validations;

using Microsoft.AspNetCore.Components.Authorization;

using Web.UI.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services
    .AddAuthorization()
    .AddAuthentication();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

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
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Web.UI.Client._Imports).Assembly);

await app.RunAsync();