using HanyCo.Infra.Internals.Data.DataSources;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InfraTestProject;

public static class ServiceCollectionExtensions
{
    public static void AddUnitTestServices(this IServiceCollection services)
        //UseConfigurationFile(services);
        => services.AddDbContext<InfraWriteDbContext>(options => options.UseInMemoryDatabase("InMemoryDb"))
                   .AddDbContext<InfraReadDbContext>(options => options.UseInMemoryDatabase("InMemoryDb"));

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