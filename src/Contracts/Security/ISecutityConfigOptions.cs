using Library.Logging;

namespace HanyCo.Infra.Security;

public interface ISecutityConfigOptions
{
    public string ConnectionString { get; set; }
    public ILogger Logger { get; set; }
    static ISecutityConfigOptions New(string connectionString) =>
        new SecutityConfigOptions { ConnectionString = connectionString };
}

internal class SecutityConfigOptions : ISecutityConfigOptions
{
    public string ConnectionString { get; set; }
    public ILogger Logger { get; set; }
}