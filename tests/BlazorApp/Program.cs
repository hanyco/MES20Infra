using Autofac;
using Autofac.Extensions.DependencyInjection;

using HanyCo.Infra;

using Library.Cqrs;
using Library.Data.SqlServer;
using Library.Mapping;

namespace BlazorApp;

public sealed class Program
{
    public static void Main(string[] args)
    {
        const string CONNECTION_STRING = "Data Source=.;Initial Catalog=MesInfra;Integrated Security=True";
        var builder = WebApplication.CreateBuilder(args);

        _ = builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
        _ = builder.Services
            .AddRazorPages()
            .AddMvcOptions(options =>
            {
                options.Filters.Add(new SampleAsyncPageFilter());
            });
        _ = builder.Services.AddServerSideBlazor();

        _ = builder.Services
                .AddOptions()
                .AddMesInfraServices<Program>(CONNECTION_STRING, Library.Logging.ILogger.Empty)
                ;

        _ = builder.Services.AddControllersWithViews(options =>
        {
            _ = options.Filters.Add<SampleActionFilter>();
        });

        _ = builder.Host.ConfigureContainer<ContainerBuilder>(builder => builder.AddCqrs(typeof(Program).Assembly));

        var app = builder.Build();

        //! Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            _ = app.UseExceptionHandler("/Error");
        }

        _ = app.UseStaticFiles();

        _ = app.UseRouting();

        _ = app.UseMesInfraMiddleware();

        _ = app.MapBlazorHub();
        _ = app.MapFallbackToPage("/_Host");

        app.Run();
    }
}