﻿using API.Extensions;
using API.Middlewares;

namespace API;

internal static class Startup
{
    public static void ConfigureApp(this WebApplication app)
    {
        _ = app
            .UseGlobalExceptionHandlerMiddleware();

        _ = app
            .UseAuthentication()
            .UseAuthorization()
            .UsePermissionMiddleware();

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

        _ = services
            .AddDatabases(configuration);

        _ = services
            .AddSecurity(configuration);

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