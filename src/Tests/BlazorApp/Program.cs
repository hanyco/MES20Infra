using Autofac;
using Autofac.Extensions.DependencyInjection;

using HanyCo.Infra;

using Library.Cqrs;
using Library.Mapping;

namespace BlazorApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var autofacServiceProviderFactory = new AutofacServiceProviderFactory();

        _ = builder.Host.UseServiceProviderFactory(autofacServiceProviderFactory);
        _ = builder.Services.AddRazorPages()
            .AddMvcOptions(options =>
            {
                options.Filters.Add(new SampleAsyncPageFilter());
            });
        _ = builder.Services.AddServerSideBlazor();

        _ = builder.Services.AddSingleton<IMapper, Mapper>()
                            .AddMesInfraServices<Program>("connection string", Library.Logging.ILogger.Empty);

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

        _ = app.UseMesInfraMiddlewares();

        _ = app.MapBlazorHub();
        _ = app.MapFallbackToPage("/_Host");

        app.Run();
    }
}