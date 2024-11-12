using Application.Features.Permissions.Services;

using Serilog;

namespace API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure Serilog to use settings from appsettings.json
        builder.Host.UseSerilog((context, services, configuration) =>
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services));

        builder.Services.ConfigureServices(builder.Configuration);

        var app = builder.Build();

        // Enable request logging for Serilog
        _ = app.UseSerilogRequestLogging(options =>
        {
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
            };
        });

        app.ConfigureApp();
        
        app.Run();
    }
}
