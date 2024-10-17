using Microsoft.AspNetCore.Hosting;

namespace API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        Startup.ConfigureServices(builder.Services, builder.Configuration);

        var app = builder.Build();
        Startup.Configure(app, app.Environment);

        app.Run();
    }    
}
