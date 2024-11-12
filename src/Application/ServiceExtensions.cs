using Application.Features.Permissions.Repositories;
using Application.Features.Permissions.Services;
using Application.Interfaces.Permissions.Repositories;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        _ = services
            .AddScoped<IAccessControlRepository, AccessControlRepository>()
            .AddScoped<IAccessControlService   , AccessControlService>();
        return services;
    }
}
