using Library.Logging;

using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HanyCo.Infra.Security.Model;

public interface ISecurityConfigOptions
{
    ILogger Logger { get; }
    Action<SqlServerDbContextOptionsBuilder>? ConnectionString { get; }
}