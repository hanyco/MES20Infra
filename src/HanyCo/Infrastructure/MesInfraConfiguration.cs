﻿using HanyCo.Infra.Web.Middlewares;

using Library.Data.Ado;
using Library.Data.SqlServer;
using Library.Mapping;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HanyCo.Infra;

public static class MesInfraConfiguration
{
    public static IServiceCollection AddMesInfraServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ApplicationConnectionString");
        return services.AddMemoryCache()
                .AddScoped(_ => new Sql(connectionString!))
                .AddScoped<AdoGenericRepository>()
                .AddSingleton<IMapper, Mapper>()
                .AddHttpContextAccessor();
    }

    public static IApplicationBuilder UseMesInfraMiddleware(this IApplicationBuilder app)
        => app.UseMiddleware<LicenseManagerMiddleware>()
            .UseMiddleware<InterceptorMiddleware>();
}