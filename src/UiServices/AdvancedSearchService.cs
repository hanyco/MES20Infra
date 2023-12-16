﻿using System.Diagnostics.CodeAnalysis;
using System.Text;

using Contracts.Services;
using Contracts.ViewModels;

using HanyCo.Infra.Markers;

using Library.CodeGeneration.Models;
using Library.Results;
using Library.Validations;

namespace Services;

[Service]
internal sealed class AdvancedSearchService : IAdvancedSearchService
{
    public Result<Codes> GenerateCodes([DisallowNull] AdvancedSearchViewModel args)
    {
        if (!args.Check().ArgumentNotNull().Any().TryParse(out var vr))
        {
            return Result<Codes>.From(vr, Codes.Empty);
        }
        var buffer = new StringBuilder();

        var statement = buffer.Build();
        var result = Code.New("Where Clause", Languages.Sql, statement);
        return result.ToCodes();
    }
}