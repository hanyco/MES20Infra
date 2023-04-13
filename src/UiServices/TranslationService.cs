using System.Globalization;

using HanyCo.Infra.Internals.Data.DataSources;

using Library.Interfaces;
using Library.Results;

using Microsoft.EntityFrameworkCore;

namespace HanyCo.Infra.UI.Services.Imp;

internal sealed class TranslationService : ITranslationService
{
    private readonly InfraReadDbContext _readDbContext;
    private readonly InfraWriteDbContext _writeDbContext;

    public TranslationService(InfraReadDbContext readDbContext, InfraWriteDbContext writeDbContext)
        => (this._writeDbContext, this._readDbContext) = (writeDbContext, readDbContext);

    public async Task<Result<string>> GetTranslationAsync(string translationKey, string? langCode = null)
    {
        langCode ??= CurrentCulture();
        var query = from x in this._readDbContext.Translations
                    where x.Key == translationKey && x.LangCode == langCode
                    select x.Value;
        var dbResult = await query.FirstOrDefaultAsync();
        return dbResult.IsNullOrEmpty() ? Result<string>.CreateFailure(value: translationKey)! : Result<string>.CreateSuccess(dbResult);
    }

    public async Task<Result> SetTranslationAsync(string translationKey, string translationValue, string? langCode = null)
    {
        langCode ??= CurrentCulture();
        var query = from x in this._writeDbContext.Translations
                    where x.Key == translationKey && x.LangCode == langCode
                    select x;
        var dbResult = await query.FirstOrDefaultAsync();
        if (dbResult != null)
        {
            dbResult.Value = translationValue;
        }
        else
        {
            var translation = new Translation { Key = translationKey, LangCode = langCode, Value = translationValue };
            _ = this._writeDbContext.Translations.Add(translation);
        }
        return await this._writeDbContext.SaveChangesResultAsync();
    }

    private static string CurrentCulture()
        => CultureInfo.CurrentUICulture.EnglishName;
}