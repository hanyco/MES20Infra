using HanyCo.Infra.CodeGen.Contracts;
using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.Security;
using HanyCo.Infra.Security.Identity;
using HanyCo.Infra.Security.Identity.Entity;
using HanyCo.Infra.Security.Model;

using Library.BusinessServices;
using Library.CodeGeneration.v2;
using Library.Interfaces;
using Library.Logging;
using Library.Mapping;
using Library.Threading.MultistepProgress;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Services;

namespace InfraTestProject;

public sealed class Startup
{
    public static void ConfigureServices(IServiceCollection services)
    {
        _ = services.AddIdentity<InfraIdentityUser, InfraIdentityRole>();

        services.AddUnitTestServices();
        var result = services.BuildServiceProvider();
        DI.Initialize(result);
        InitializeDatabase();
    }

    //public static IServiceProvider GetServiceProvider()
    //{
    //    var services = new ServiceCollection();
    //    services.AddUnitTestServices();
    //    var result = services.BuildServiceProvider();
    //    DI.Initialize(result);
    //    return result;
    //}

    private static void InitializeDatabase()
    {
        var db = DI.GetService<InfraWriteDbContext>();
        _ = db.Database.EnsureDeleted();
        _ = db.Database.EnsureCreated();
        _ = db.Modules.Add(new() { Guid = Guid.NewGuid(), Name = "Unit Test Module 1" });
        _ = db.Modules.Add(new() { Guid = Guid.NewGuid(), Name = "Unit Test Module 2" });
        _ = db.SaveChanges();
    }
}

internal static class ServiceCollectionExtensions
{
    public static void AddUnitTestServices(this IServiceCollection services)
    {
        _ = services
                .RegisterServices<IService>(typeof(ContractsModule), typeof(ServicesModule));

        _ = services
                .AddScoped<IMapper, Mapper>()
                .AddScoped<Library.Logging.ILogger, EmptyLogger>()
                .AddSingleton(IProgressReport.New())
                .AddScoped<ICodeGeneratorEngine, RoslynCodeGenerator>();

        var inMemoryDatabaseRoot = new InMemoryDatabaseRoot();
        _ = services
                .AddDbContext<InfraWriteDbContext>(options => options.UseInMemoryDatabase("MesInfra", inMemoryDatabaseRoot)
                                                                     .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning)))
                .AddDbContext<InfraReadDbContext>(options => options.UseInMemoryDatabase("MesInfra", inMemoryDatabaseRoot));
    }
}