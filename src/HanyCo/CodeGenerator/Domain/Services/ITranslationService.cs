using Library.Interfaces;
using Library.Results;

namespace HanyCo.Infra.CodeGen.Contracts.Services;

public interface ITranslationService : IService
{
    public Task<Result<string>> GetTranslationAsync(string translationKey, string? langCode = null, CancellationToken token = default);

    public Task<Result> SetTranslationAsync(string translationKey, string translationValue, string? langCode = null, CancellationToken token = default);
}