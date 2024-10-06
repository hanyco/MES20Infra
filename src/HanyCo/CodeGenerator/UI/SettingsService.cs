using System.Diagnostics;
using System.IO;
using System.Text.Json;

using HanyCo.Infra.CodeGen.Domain.ViewModels;

using Library.Data.SqlServer.Builders;
using Library.Validations;

namespace HanyCo.Infra.UI;

internal static class SettingsService
{
    private static SettingsModel? _settings;

    public static SettingsModel Get() =>
        _settings ??= JsonSerializer.Deserialize<SettingsModel>(File.ReadAllText(GetSettingFilePath())) ?? new SettingsModel();

    public static SettingsModel Save(this SettingsModel settings)
    {
        _ = settings.Check().NotNull().NotNull(x => x.connectionString).ThrowOnFail();
        _ = ConnectionStringBuilder.Validate(settings.connectionString).ThrowOnFail();

        _settings = settings;
        File.WriteAllText(GetSettingFilePath(), JsonSerializer.Serialize(_settings));
        return _settings;
    }

    private static string GetSettingFilePath() =>
        Path.Combine(Environment.CurrentDirectory, $"AppSettings.{(Debugger.IsAttached ? "Development" : "Production")}.json");
}