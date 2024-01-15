using Library.Logging;

namespace HanyCo.Infra.Security.Model;

public interface ISecurityConfigOptions
{
    string ConnectionString { get; }
    ILogger Logger { get; }

    static ISecurityConfigOptions New(string connectionString, ILogger? logger = null)
        => new SecurityConfigOptions(connectionString, logger);
}

public class JwtOptions()
{
    public string Audience { get; set; } = default!;
    public DateTime ExpirationDate { get; set; } = default!;
    public string Issuer { get; set; } = default!;
    public string SecretKey { get; set; } = default!;
}

internal sealed record class SecurityConfigOptions(string ConnectionString, ILogger Logger) : ISecurityConfigOptions;