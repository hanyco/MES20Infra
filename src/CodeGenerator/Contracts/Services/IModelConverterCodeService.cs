using Contracts.ViewModels;

using Library.CodeGeneration.Models;
using Library.Interfaces;
using Library.Results;

namespace Contracts.Services;

public interface IModelConverterCodeService : IBusinessService
{
    Result<Codes> GenerateCode(DtoViewModel src, string dstClassName, string methodName);
}