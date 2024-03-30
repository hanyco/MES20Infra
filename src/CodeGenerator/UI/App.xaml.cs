using System.Diagnostics;
using System.IO;
using System.Windows;

using HanyCo.Infra;
using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.UI;
using HanyCo.Infra.UI.Controls.Pages;
using HanyCo.Infra.UI.Dialogs;
using HanyCo.Infra.UI.Pages;

using Library.CodeGeneration.v2;
using Library.Data.EntityFrameworkCore;
using Library.EventsArgs;
using Library.Interfaces;
using Library.Threading.MultistepProgress;
using Library.Validations;
using Library.Wpf.Windows;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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

    public new static App Current => LibApp.Current.Cast().As<App>()!;

    protected override void OnConfigureServices(ServiceCollection services)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

        var connectionString = config.GetConnectionString("ApplicationConnectionString").NotNull();
        var settings = SettingsService.Get()
            .With(x => x.connectionString = connectionString)
            .Save();

        addBclServices(services);
        addDataContext(services, settings);
        addLogger(services);
        addMesInfraServices(services, config);
        registerServices(services);
        addPages(services);

        return;

        static void addBclServices(IServiceCollection services) =>
            services.AddScoped<ICodeGeneratorEngine, RoslynCodeGenerator>();

        static void addDataContext(IServiceCollection services, SettingsModel settings)
        {
            _ = services
                .AddDbContext<InfraWriteDbContext>(applyDefaults)
               .AddDbContext<InfraReadDbContext>(options =>
               {
                   applyDefaults(options);
                   _ = options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
               });

            void applyDefaults(DbContextOptionsBuilder options)
                => options
                    .UseSqlServer(settings.connectionString!)
                    .LogTo(Current.LogEf)
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

        static void addMesInfraServices(IServiceCollection services, IConfiguration configuration)
            => services.AddMesInfraServices(configuration);
    }

    private void InitializeLog()
    {
        this._logFilePath = Path.Combine(Environment.CurrentDirectory, $"{this.Title ?? "Application"}.log");
        this.Logger.LogLevel = Library.Logging.LogLevel.Trace;
        this.Logger.Logging += this.Logger_Logging;
        this.Logger.Debug("Loading Main Window...");
    }

    [DebuggerStepThrough]
    private void LogEf(string message)
        => this.Logger.Log(message, Library.Logging.LogLevel.Debug);

    private void Logger_Logging(object? sender, ItemActedEventArgs<LogRecord<object>> e)
        => Debug.WriteLine(e.Item.Message);

    private void OnStartup(object sender, StartupEventArgs e)
    {
        this.InitializeLog();
        var mainWindow = DI.GetService<MainWindow>();
        mainWindow!.Show();
    }
}