using Library.Logging;

namespace HanyCo.Infra.Security.Model;

public interface ISecurityConfigOptions
{
    string? ConnectionString { get; }
    ILogger Logger { get; }

    static ISecurityConfigOptions New(string? connectionString, ILogger? logger = null)
        => new SecurityConfigOptions(connectionString, logger);
}

internal sealed class SecurityConfigOptions(string? connectionString, ILogger? logger) : ISecurityConfigOptions
{
    public string? ConnectionString { get; } = connectionString;
    public ILogger Logger { get; } = logger ?? ILogger.Empty;
}