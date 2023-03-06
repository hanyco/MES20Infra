
using System.IO;

using Autofac.Extensions.DependencyInjection;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ServiceHost
{
    public static class Program
    {
        public static void Main(string[] args) =>
            CreateHostBuilder(args)
            .Build()
            .Run();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory<Autofac.ContainerBuilder>(new AutofacServiceProviderFactory())
            .ConfigureServices(services => services.AddAutofac())
            .ConfigureWebHostDefaults(webBuilder =>
            {
                _ = webBuilder.UseStartup<Startup>();
                _ = webBuilder.ConfigureServices((context, services) =>
                  {
                      var config = context.Configuration.GetSection("Kestrel");
                      _ = services.Configure<KestrelServerOptions>(config);
                  })
                .UseKestrel();
                _ = webBuilder.ConfigureAppConfiguration((_, config) => config.SetBasePath(Directory.GetCurrentDirectory()));
            })
            .ConfigureLogging((hostingContext, logging) =>
            {
                _ = logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging")).AddConsole().AddDebug();
            });
    }
}
