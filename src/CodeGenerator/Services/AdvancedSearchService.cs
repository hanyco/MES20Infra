using System.Diagnostics.CodeAnalysis;
using System.Text;

using Library.CodeGeneration.Models;
using Library.Interfaces;
using Library.Results;
using Library.Validations;

namespace Services.CodeGen;

internal sealed class AdvancedSearchService : IBusinessService, IAdvancedSearchService
{
    public Result<Codes> GenerateCodes([DisallowNull] AdvancedSearchViewModel args)
    {
        if (!args.Check().ArgumentNotNull().Any().TryParse(out var vr))
        {
            return Result.From(vr, Codes.Empty);
        }
        var buffer = new StringBuilder();

        var statement = buffer.Build();
        var result = Code.New("Where Clause", Languages.Sql, statement);
        return result.ToCodes();
    }
}