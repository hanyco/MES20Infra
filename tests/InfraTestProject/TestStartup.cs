using HanyCo.Infra.Internals.Data.DataSources;

using Library.Interfaces;
using Library.Logging;
using Library.Mapping;
using Library.Threading.MultistepProgress;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

using UiContracts;

using UiServices;

namespace InfraTestProject;

public static class TestStartup
{
    public static IServiceProvider GetServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddUnitTestServices();
        var result = services.BuildServiceProvider();
        DI.Initialize(result);
        return result;
    }
}

internal static class ServiceCollectionExtensions
{
    public static void AddUnitTestServices(this IServiceCollection services)
    {
        var inMemoryDatabaseRoot = new InMemoryDatabaseRoot();
        _ = services
                .RegisterServices<IService>(typeof(ContarctsModule), typeof(ServicesModule))
                .AddScoped<IMapper, Mapper>()
                .AddScoped<ILogger, EmptyLogger>()
                .AddDbContext<InfraWriteDbContext>(options => options.UseInMemoryDatabase("MesInfra", inMemoryDatabaseRoot)
                                                                     .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning)))
                .AddDbContext<InfraReadDbContext>(options => options.UseInMemoryDatabase("MesInfra", inMemoryDatabaseRoot))
                .AddSingleton(IMultistepProcess.New());
    }
}