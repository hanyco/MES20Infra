using Contracts.ViewModels;

using Library.CodeGeneration;
using Library.CodeGeneration.Models;
using Library.Interfaces;
using Library.Results;

namespace Contracts.Services;

public interface ISqlStatementCodeGenerator : IBusinessService
{
    CodeGeneratorResult GenerateSelectAllSqlStatement(DtoViewModel dto, string codeName);
}