using HanyCo.Infra.Internals.Data.DataSources;

using Library.Helpers;
using Library.Interfaces;
using Library.Logging;
using Library.Mapping;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using UiContracts;

using UiServices;

namespace InfraTestProject;

internal static class ServiceCollectionExtensions
{
    public static void AddUnitTestServices(this IServiceCollection services)
    {
        var inMemoryDatabaseRoot = new InMemoryDatabaseRoot();
        services
                    .RegisterServices<IService>(typeof(ContarctsModule), typeof(ServicesModule))
                    .AddScoped<IMapper, Mapper>()
                    .AddScoped<ILogger, EmptyLogger>()
                    .AddDbContext<InfraWriteDbContext>(options => options.UseInMemoryDatabase("MesInfra", inMemoryDatabaseRoot)
                                                                         .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning)))
                    .AddDbContext<InfraReadDbContext>(options => options.UseInMemoryDatabase("MesInfra", inMemoryDatabaseRoot));
    }

    private static void UseConfigurationFile(IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();

        var serviceSettings = configuration.GetSection("Services").Get<ServiceSettings[]>();

        foreach (var serviceSetting in serviceSettings)
        {
            if (serviceSetting?.Implementation == null)
            {
                continue;
            }

            var serviceType = Type.GetType(serviceSetting.Implementation);
            if (serviceType == null)
            {
                continue;
            }

            _ = serviceSetting.Type switch
            {
                "Transient" => services.AddTransient(serviceType),
                "Scoped" => services.AddScoped(serviceType),
                "Singleton" => services.AddSingleton(serviceType),
                _ => throw new Exception($"Invalid service type: {serviceSetting.Type}"),
            };
        }
    }
}

file class ServiceSettings
{
    public string? Implementation { get; set; }

    public string? Name { get; set; }

    public string? Type { get; set; }
}