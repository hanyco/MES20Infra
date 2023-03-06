using Microsoft.Extensions.DependencyInjection;

namespace InfraTestProject;

public static class TestStartup
{
    public static IServiceProvider GetServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddUnitTestServices();
        return services.BuildServiceProvider();
    }
}