using HanyCo.Infra.Security;

using Library.Coding;
using Library.Logging;
using Library.Web.Middlewares;
using Library.Web.Middlewares.Infra;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace HanyCo.Infra;

public static class MesInfraConfiguration
{
    public static IServiceCollection AddMesInfraServices<TStartup>(this IServiceCollection services, string connectionString, ILogger logger)
        => services.AddMemoryCache()
            .AddHttpContextAccessor()
            //.AddMesInfraSecurityServices<TStartup>(ISecurityConfigOptions.New(connectionString).With(x => x.Logger = logger))
            ;

    public static IApplicationBuilder UseMesInfraMiddleware(this IApplicationBuilder app)
        => app.UseMiddleware<ExceptionHandlerMiddleware>()
              .UseMiddleware<LicenseManagerMiddleware>()
              .UseMiddleware<InterceptorMiddleware>()
              //.UseMesSecurityInfraMiddleware()
        ;
}