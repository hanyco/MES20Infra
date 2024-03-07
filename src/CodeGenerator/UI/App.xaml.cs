using System.IO;
using System.Windows;

using HanyCo.Infra;
using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.UI;
using HanyCo.Infra.UI.Controls.Pages;
using HanyCo.Infra.UI.Dialogs;
using HanyCo.Infra.UI.Pages;

using Library.BusinessServices;
using Library.CodeGeneration.v2;
using Library.EventsArgs;
using Library.Interfaces;
using Library.Threading.MultistepProgress;
using Library.Validations;
using Library.Wpf.Windows;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Services;

namespace UI;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : LibApp
{
    private string? _logFilePath;

    protected override void OnConfigureServices(ServiceCollection services)
    {
        var settings = SettingsService.Get();
        Check.MustBeArgumentNotNull(settings.connectionString);

        addBclServices(services);
        addDataContext(services, settings);
        addLogger(services);
        addMesInfraServices(services, settings);
        registerServices(services);
        addPages(services);

        return;

        static void addBclServices(IServiceCollection services) =>
            services.AddScoped<ICodeGeneratorEngine, RoslynCodeGenerator>();

        static void addDataContext(IServiceCollection services, SettingsModel settings)
        {
            services
#if !DEBUG_UNIT_TEST
               .AddDbContext<InfraWriteDbContext>(applyDefaults)
               .AddDbContext<InfraReadDbContext>(options =>
               {
                   applyDefaults(options);
                   _ = options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
               })

#endif
#if TEST_MODE
               .AddDbContext<InfraWriteDbContext>(options =>
               {
                   _ = options.UseSqlServer(settings.connectionString!);
                   _ = options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                   _ = options.EnableSensitiveDataLogging();
               })
               .AddDbContext<InfraReadDbContext>(options =>
               {
                   _ = options.UseSqlServer();
                   _ = options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                   _ = options.EnableSensitiveDataLogging();
               })
#endif
               ;

            void applyDefaults(DbContextOptionsBuilder options)
                => options
                    .UseSqlServer(settings.connectionString!)
                    .LogTo(Console.WriteLine)
                    .EnableDetailedErrors()
                    .EnableSensitiveDataLogging()
                    //.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null)
                    ;
        }

        static void addLogger(IServiceCollection services) => _ = services
                .AddSingleton<Library.Logging.ILogger>(AppLogger)
                .AddSingleton<Microsoft.Extensions.Logging.ILogger>(new WebLogger(AppLogger))
                .AddLogging(builder => builder.AddConsole())
                .AddSingleton(IProgressReport.New());

        static void registerServices(IServiceCollection services)
            => services
                .RegisterServices<IService>(typeof(ContractsModule), typeof(ServicesModule))
                .RegisterServicesWithIService<App>();

        static void addPages(IServiceCollection services)
        {
            _ = services
               .AddSingleton<MainWindow>()
               .AddTransient<SettingsPage>()
               .AddTransient<DtoDetailsPage>()
               .AddTransient<CqrsQueryDetailsPage>()
               .AddTransient<CqrsCommandDetailsPage>()
               .AddTransient<CodeDetailsPage>()
               .AddTransient<CreateTableCrudPage>()
               .AddTransient<BlazorComponentGenertorPage>()
               .AddTransient<BlazorPageGeneratorPage>()
               .AddTransient<FunctionalityEditorPage>();

            _ = services.AddTransient<SelectCqrsDialog>();
        }

        static void addMesInfraServices(IServiceCollection services, SettingsModel settings)
            => services.AddMesInfraServices(settings.connectionString!, AppLogger);
    }

    private void Logger_Logging(object? sender, ItemActedEventArgs<object> e)
    {
    }

    private void OnStartup(object sender, StartupEventArgs e)
    {
        this.InitializeLog();
        var mainWindow = DI.GetService<MainWindow>();
        mainWindow!.Show();
    }

    private void InitializeLog()
    {
        this._logFilePath = Path.Combine(Environment.CurrentDirectory, $"{this.Title ?? "Application"}.log");
        this.Logger.LogLevel = Library.Logging.LogLevel.Trace;
        this.Logger.Logging += this.Logger_Logging;
        this.Logger.Debug("Loading Main Window...");
    }
}