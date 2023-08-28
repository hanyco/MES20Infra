using HanyCo.Infra.Internals.Data.DataSources;

using InfraTestProject.Tests;

using Library.BusinessServices;
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

public sealed class Startup
{
    public static IServiceProvider GetServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddUnitTestServices();
        var result = services.BuildServiceProvider();
        DI.Initialize(result);
        return result;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddUnitTestServices();
        var result = services.BuildServiceProvider();
        DI.Initialize(result);
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        var db = DI.GetService<InfraWriteDbContext>();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
        db.Modules.Add(new() { Guid = Guid.NewGuid(), Name = "Unit Test Module 1"});
        db.Modules.Add(new() { Guid = Guid.NewGuid(), Name = "Unit Test Module 2" });
        db.SaveChanges();
    }
}

internal static class ServiceCollectionExtensions
{
    public static void AddUnitTestServices(this IServiceCollection services)
    {
        _ = services
                .RegisterServices<IService>(typeof(ContarctsModule), typeof(ServicesModule));
        
        _ = services
                .AddScoped<IMapper, Mapper>()
                .AddScoped<ILogger, EmptyLogger>()
                .AddSingleton(IProgressReport.New());
        
        var inMemoryDatabaseRoot = new InMemoryDatabaseRoot();
        _ = services
                .AddDbContext<InfraWriteDbContext>(options => options.UseInMemoryDatabase("MesInfra", inMemoryDatabaseRoot)
                                                                     .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning)))
                .AddDbContext<InfraReadDbContext>(options => options.UseInMemoryDatabase("MesInfra", inMemoryDatabaseRoot));
    }
}