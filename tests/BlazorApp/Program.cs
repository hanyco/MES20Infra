using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace BlazorApp;
public class Program
{
    public static async Task Main(string[] args)
    {
        const string CONNECTION_STRING = "Data Source=.;Initial Catalog=MesInfra;Integrated Security=True";
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

        MesConfig(builder);

        await builder.Build().RunAsync();
    }

    private static void MesConfig(WebAssemblyHostBuilder builder)
    {
        
    }
}
