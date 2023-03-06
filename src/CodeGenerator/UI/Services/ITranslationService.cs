using Library.Interfaces;
using Library.Results;

namespace HanyCo.Infra.UI.Services;

public interface ITranslationService : IService
{
    public Task<Result<string>> GetTranslationAsync(string translationKey, string? langCode = null);

    public Task<Result> SetTranslationAsync(string translationKey, string translationValue, string? langCode = null);
}