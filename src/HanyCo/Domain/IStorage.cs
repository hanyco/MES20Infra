using System.Diagnostics.CodeAnalysis;

using Library.Results;

namespace HanyCo.Infra;

public interface IStorage
{
    Task<Result> DeleteAsync([DisallowNull] string key, CancellationToken cancellationToken = default);

    Task<Result<string>> LoadAsync([DisallowNull] string key, CancellationToken cancellationToken = default);

    Task<Result> SaveAsync([DisallowNull] string key, string value, CancellationToken cancellationToken = default);
}