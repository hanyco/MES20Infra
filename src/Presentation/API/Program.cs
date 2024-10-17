
using API.Extensions;

using Library.Data.SqlServer;
using Library.Validations;

using Microsoft.AspNetCore.Hosting;

namespace API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ConfigureServices(builder.Services, builder.Configuration);

        var app = builder.Build();
        Configure(app, app.Environment);

        app.Run();
    }

    private static void ConfigureServices(IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddScoped(_ => new Sql(configuration.GetConnectionString("ApplicationConnectionString").NotNull(() => "Connection String not found.")));
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly));
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(DomainModule).Assembly));
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(ApplicationModule).Assembly));
        services.AddContextInfrastructure(configuration);
        services.AddSharedInfrastructure(configuration);
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    private static void Configure(WebApplication app, IWebHostEnvironment environment)
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
