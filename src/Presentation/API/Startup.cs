
using API.Extensions;

using Library.Data.SqlServer;
using Library.Validations;

namespace API;
class Startup
{
    public static void ConfigureServices(IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly))
            .AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(DomainModule).Assembly))
            .AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(ApplicationModule).Assembly));
        
        services.AddContextInfrastructure(configuration)
            .AddSharedInfrastructure(configuration);
        
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddEssentials();
    }

    public static void Configure(WebApplication app, IWebHostEnvironment environment)
    {
        // Configure the HTTP request pipeline.
        if (environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
    }
}