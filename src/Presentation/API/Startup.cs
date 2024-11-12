using API.Extensions;
using API.Middlewares;

namespace API;

internal static class Startup
{
    public static void ConfigureApp(this WebApplication app)
    {
        _ = app
            .UseGlobalExceptionHandlerMiddleware();

        Console.WriteLine("Adding AccessControlMiddleware to the pipeline...");
        _ = app
            .UseAuthentication()
            .UseAuthorization()
            .UseAccessControlMiddleware();
        Console.WriteLine("AccessControlMiddleware added successfully.");

        _ = app
            .MapControllers();

        _ = app
            .UseSwagger()
            .UseSwaggerUI();
    }

    public static void ConfigureServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        _ = services
            .AddMediatR();

        Console.WriteLine("Adding databases...");
        _ = services
            .AddDatabases(configuration);
        Console.WriteLine("Databases added.");

        Console.WriteLine("Adding security...");
        _ = services
            .AddSecurity(configuration);
        Console.WriteLine("Security added.");

        _ = services
            .AddSharedInfrastructure(configuration);

        _ = services
            .AddAddControllers();

        _ = services
            .AddHttpContextAccessor();

        _ = services
            .AddSwagger();
    }
}