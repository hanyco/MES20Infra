﻿using HanyCo.Infra.CodeGen.Contracts.CodeGen.ViewModels;
using HanyCo.Infra.CodeGen.Contracts.ViewModels;

using Library.CodeGeneration;
using Library.CodeGeneration.Models;
using Library.Interfaces;
using Library.Results;

namespace HanyCo.Infra.CodeGen.Contracts.Services;

public interface ISqlStatementCodeGenerator : IBusinessService
{
    CodeGeneratorResult GenerateSelectAllSqlStatement(DtoViewModel dto, string codeName);
}