using System.Diagnostics.CodeAnalysis;

using HanyCo.Infra;

using Library.Results;

namespace BlazorApp.Services;

public sealed class SessionStorage(Blazored.SessionStorage.ISessionStorageService sessionStorage) : IStorage
{
    public async Task<Result> DeleteAsync([DisallowNull] string key, CancellationToken cancellationToken = default)
    {
        await sessionStorage.RemoveItemAsync(key, cancellationToken);
        return Result.Success;
    }

    public async Task<Result<string>> LoadAsync([DisallowNull] string key, CancellationToken cancellationToken = default)
    {
        var result = await sessionStorage.GetItemAsStringAsync(key, cancellationToken);
        return Result<string>.CreateSuccess(result);
    }

    public async Task<Result> SaveAsync([DisallowNull] string key, string value, CancellationToken cancellationToken = default)
    {
        await sessionStorage.SetItemAsStringAsync(key, value, cancellationToken);
        return Result.Success;
    }
}