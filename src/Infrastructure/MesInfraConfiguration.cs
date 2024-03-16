using HanyCo.Infra.Security;
using HanyCo.Infra.Security.Model;
using HanyCo.Infra.Web.Middlewares;

using Library.Data.SqlServer;
using Library.Logging;
using Library.Mapping;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HanyCo.Infra;

public static class MesInfraConfiguration
{
    public static IServiceCollection AddMesInfraServices(this IServiceCollection services, IConfiguration configuration, ILogger logger)
    {
        var connectionString = configuration.GetConnectionString("ApplicationConnectionString");
        return services.AddMemoryCache()
                .AddSingleton(new Sql(connectionString))
                .AddSingleton<IMapper, Mapper>()
                .AddHttpContextAccessor()
                .AddMesInfraSecurityServices(ISecurityConfigOptions.New(connectionString, logger))
                ;
    }

    public static IApplicationBuilder UseMesInfraMiddleware(this IApplicationBuilder app)
        => app.UseMiddleware<ExceptionHandlerMiddleware>()
            .UseMiddleware<LicenseManagerMiddleware>()
            .UseMiddleware<InterceptorMiddleware>()
            .UseMesSecurityInfraMiddleware()
            ;
}