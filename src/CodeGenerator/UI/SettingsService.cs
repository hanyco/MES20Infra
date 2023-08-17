using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;

using Contracts.ViewModels;

using Library.Data.SqlServer.Builders;
using Library.Validations;
using Library.Wpf.Helpers;

namespace HanyCo.Infra.UI.Services.Imp;

internal static class SettingsService
{
    private static SettingsModel? _settings;

    public static SettingsModel Get()
    {
        _settings ??= Load();
        return _settings;
    }

    [ModuleInitializer]
    public static void Initialize()
    {
        if (!ControlHelper.IsDesignTime() && _settings is null)
        {
            _settings = Load();
        }
    }

    public static Task SaveAsync(SettingsModel settings)
    {
        Validate(settings);
        _settings = settings;
        var settingFilePath = $"AppSettings.{(IsDevelopment() ? "Development" : "Production")}.json";
        var settingText = JsonSerializer.Serialize(_settings);
        return File.WriteAllTextAsync(settingFilePath, settingText);
    }

    private static bool IsDevelopment()
        => Debugger.IsAttached;

    private static SettingsModel Load()
    {
        //x var settingFilePath = $"AppSettings.{(IsDevelopment() ? "Development" : "Production")}.json";
        var settingFilePath = $"AppSettings{(IsDevelopment() ? "" : ".Production")}.json";
        var settingText = File.ReadAllText(settingFilePath);
        return JsonSerializer.Deserialize<SettingsModel>(settingText) ?? new SettingsModel();
    }

    private static void Validate(SettingsModel settings)
    {
        _ = settings.Check(CheckBehavior.ThrowOnFail).NotNull().NotNull(x => x.connectionString);
        _ = ConnectionStringBuilder.Validate(settings.connectionString);
    }
}