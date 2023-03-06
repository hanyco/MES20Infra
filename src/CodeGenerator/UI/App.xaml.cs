using System.IO;
using System.Windows;

using Contracts.Services;

using HanyCo.Infra;
using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.UI.Controls.Pages;
using HanyCo.Infra.UI.Dialogs;
using HanyCo.Infra.UI.Pages;
using HanyCo.Infra.UI.Services;
using HanyCo.Infra.UI.Services.Imp;

using Library.EventsArgs;
using Library.Interfaces;
using Library.Threading.MultistepProgress;
using Library.Validations;
using Library.Wpf.Windows;
using Library.Wpf.Windows.UI;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using UiContracts;

using UiServices;

using Windows.UI.Notifications;

namespace UI;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : LibApp
{
    #region Fields

    private string? _logFilePath;

    #endregion Fields

    protected override void OnConfigureServices(ServiceCollection services)
    {
        var settings = SettingsService.Get();
        Check.IfArgumentNotNull(settings.connectionString);

        addDataContext(services, settings);
        addLogger(services);
        addMesInfraServices(services, settings);
        registerServices(services);
        addPages(services);

        return;

        static void addDataContext(IServiceCollection services, HanyCo.Infra.UI.ViewModels.SettingsModel settings)
            => services
#if !TEST_MODE
               .AddDbContext<InfraWriteDbContext>(options =>
               {
                   _ = options.UseSqlServer(settings.connectionString!);
                   _ = options.EnableSensitiveDataLogging();
               })
#endif
               .AddDbContext<InfraReadDbContext>(options =>
               {
                   _ = options.UseSqlServer(settings.connectionString!);
                   _ = options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                   _ = options.EnableSensitiveDataLogging();
               })

#if TEST_MODE
               .AddDbContext<InfraWriteDbContext>(options =>
               {
                   _ = options.UseSqlServer(settings.connectionString!);
                   _ = options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                   _ = options.EnableSensitiveDataLogging();
               })
#endif
               ;

        static void addLogger(IServiceCollection services)
        {
            _ = services
                .AddSingleton<ILogger>(AppLogger)
                .AddSingleton<Microsoft.Extensions.Logging.ILogger>(new WebLogger(AppLogger))
                .AddSingleton(IMultistepProcess.New());
        }

        static void registerServices(IServiceCollection services)
            => services
                .RegisterServices<IService>(typeof(ContarctsModule), typeof(ServicesModule))
                .RegisterServicesWithIService<App>()
                .AddScoped<IEntityViewModelConverter, EntityViewModelConverter>()
                .AddScoped<IBlazorCodingService, BlazorCodingService>()
                .AddScoped<IBlazorComponentService, BlazorComponentService>()
                .AddScoped<IBlazorPageService, BlazorPageService>();

        static void addPages(IServiceCollection services)
        {
            _ = services
               .AddSingleton<MainWindow>()
               .AddTransient<DtoDetailsPage>()
               .AddTransient<CqrsQueryDetailsPage>()
               .AddTransient<CqrsCommandDetailsPage>()
               .AddTransient<SettingsPage>()
               .AddTransient<CodeDetailsPage>()
               .AddTransient<CreateTableCrudPage>()
               .AddTransient<BlazorComponentGenertorPage>()
               .AddTransient<BlazorPageGeneratorPage>()
               .AddTransient<SecurityDescriptorEditorPage>()
               .AddTransient<FunctionalityEditorPage>();

            _ = services.AddTransient<SelectCqrsDialog>();
        }

        static void addMesInfraServices(IServiceCollection services, HanyCo.Infra.UI.ViewModels.SettingsModel settings)
            => services.AddMesInfraServices<App>(settings.connectionString!, AppLogger);
    }

    private async void Logger_Logging(object? sender, ItemActedEventArgs<object> e)
    {
        if (e.Item is not LogRecord<object> log || log.Message?.ToString() is not string message)
        {
            return;
        }

        if (log.Level is LogLevel.Error or LogLevel.Fatal)
        {
            Toast.CreateLongContent(message, log.Sender?.ToString(), this.Title!).SetPriority(ToastNotificationPriority.High).Show();
        }
        else if (log.Level.MeetsLevel(LogLevel.Info))
        {
            Toast.CreateLongContent(message, this.Title!).Show();
        }

        if (this._logFilePath is not null)
        {
            if (log.Level.MeetsLevel(LogLevel.Fatal))
            {
                using var sw = new StreamWriter(this._logFilePath, true);
                try
                {
                    await sw.WriteLineAsync(LoggingHelper.Reformat(log));
                }
                finally
                {
                    sw.Close();
                }
            }
        }
    }

    private void OnStartup(object sender, StartupEventArgs e)
    {
        this._logFilePath = Path.Combine(Environment.CurrentDirectory, $"{this.Title ?? "Application"}.log.txt");
        this.Logger.LogLevel = LogLevel.Trace;
        this.Logger.Logging += this.Logger_Logging;
        this.Logger.Debug("Loading Main Window...");
        var mainWindow = DI.GetService<MainWindow>();
        mainWindow!.Show();
    }
}