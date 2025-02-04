﻿using System.Diagnostics.CodeAnalysis;
using System.Text;

using HanyCo.Infra.CodeGen.Domain.Services;
using HanyCo.Infra.CodeGen.Domain.ViewModels;

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

        var statement = buffer.ToString();
        var result = Code.New("Where Clause", Languages.Sql, statement);
        return result.ToCodes();
    }
}