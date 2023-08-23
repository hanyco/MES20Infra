using System.Diagnostics;
using System.IO;
using System.Text.Json;

using Contracts.ViewModels;

using Library.Data.SqlServer.Builders;
using Library.Results;
using Library.Validations;

namespace HanyCo.Infra.UI;

internal static class SettingsService
{
    private static SettingsModel? _settings;

    public static SettingsModel Get()
    {
        _settings ??= Load();
        return _settings;
    }

    public static Task SaveAsync(this SettingsModel settings)
    {
        _ = Validate(settings).ThrowOnFail();
        _settings = settings;
        var settingFilePath = GetSettingFilePath();
        var settingText = JsonSerializer.Serialize(_settings);
        return File.WriteAllTextAsync(settingFilePath, settingText);

        static Result Validate(SettingsModel settings) => 
            Result.Combine(
                settings.Check().NotNull().NotNull(x => x.connectionString).Build(),
                ConnectionStringBuilder.Validate(settings.connectionString)
            );
    }

    private static string GetSettingFilePath() =>
        Path.Combine(Environment.CurrentDirectory, $"AppSettings.{(IsDevelopment() ? "Development" : "Production")}.json");

    private static bool IsDevelopment() =>
        Debugger.IsAttached;

    private static SettingsModel Load()
    {
        var settingFilePath = GetSettingFilePath();
        var settingText = File.ReadAllText(settingFilePath);
        return JsonSerializer.Deserialize<SettingsModel>(settingText) ?? new SettingsModel();
    }
}