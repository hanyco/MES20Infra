﻿using HanyCo.Infra.CodeGen.Domain.ViewModels;

using Library.CodeGeneration;
using Library.CodeGeneration.Models;
using Library.Interfaces;
using Library.Results;

namespace HanyCo.Infra.CodeGen.Domain.Services;

public interface ISqlStatementCodeGenerator : IBusinessService
{
    CodeGeneratorResult GenerateSelectAllSqlStatement(DtoViewModel dto, string codeName);
}