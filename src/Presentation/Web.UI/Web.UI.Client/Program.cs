using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Web.UI.Client;

internal class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);

        var app = builder.Build();

        await app.RunAsync();
    }
}