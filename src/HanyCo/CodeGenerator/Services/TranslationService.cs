using System.Globalization;

using HanyCo.Infra.CodeGen.Domain.Services;
using HanyCo.Infra.Internals.Data.DataSources;

using Library.Results;

using Microsoft.EntityFrameworkCore;

namespace Services;

internal sealed class TranslationService : ITranslationService
{
    private readonly InfraReadDbContext _readDbContext;
    private readonly InfraWriteDbContext _writeDbContext;

    public TranslationService(InfraReadDbContext readDbContext, InfraWriteDbContext writeDbContext)
        => (this._writeDbContext, this._readDbContext) = (writeDbContext, readDbContext);

    public async Task<Result<string>> GetTranslationAsync(string translationKey, string? langCode = null, CancellationToken token = default)
    {
        langCode ??= _currentCulture();
        var query = from x in this._readDbContext.Translations
                    where x.Key == translationKey && x.LangCode == langCode
                    select x.Value;
        var dbResult = await query.FirstOrDefaultAsync(cancellationToken: token);
        return dbResult.IsNullOrEmpty() ? Result.Fail<string>(value: translationKey)! : Result.Success<string>(dbResult);
    }

    public async Task<Result> SetTranslationAsync(string translationKey, string translationValue, string? langCode = null, CancellationToken token = default)
    {
        langCode ??= _currentCulture();
        var query = from x in this._writeDbContext.Translations
                    where x.Key == translationKey && x.LangCode == langCode
                    select x;
        var dbResult = await query.FirstOrDefaultAsync(cancellationToken: token);
        if (dbResult != null)
        {
            dbResult.Value = translationValue;
        }
        else
        {
            var translation = new Translation { Key = translationKey, LangCode = langCode, Value = translationValue };
            _ = this._writeDbContext.Translations.Add(translation);
        }
        return await this._writeDbContext.SaveChangesResultAsync(cancellationToken: token);
    }

    private static string _currentCulture()
                => CultureInfo.CurrentUICulture.EnglishName;
}