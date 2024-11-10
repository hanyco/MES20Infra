
using API.Extensions;
using API.Middlewares;

using Application.Features.Permissions.Services;
using Application.Interfaces.Permissions.Repositories;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace API;

internal class Startup
{
    public static void Configure(WebApplication app, IWebHostEnvironment environment)
    {
        // Configure the HTTP request pipeline.
        if (environment.IsDevelopment())
        {
            _ = app
                .UseSwagger()
                .UseSwaggerUI();
        }

        _ = app
            .UseAuthentication()
            .UseAuthorization();

        _ = app
            .UseGlobalExceptionHandlerMiddleware()
            .UseAccessControlMiddleware();

        _ = app
            .MapControllers();
    }

    public static void ConfigureServices(IServiceCollection services, ConfigurationManager configuration)
    {
        _ = services
            .AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly))
            .AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(DomainModule).Assembly))
            .AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(ApplicationModule).Assembly));

        _ = services
            .AddContextInfrastructure(configuration)
            .AddSharedInfrastructure(configuration);

        _ = services
            .AddControllersWithViews(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });
        _ = services
            .AddEndpointsApiExplorer();
        _ = services
            .AddEssentials();
    }
}