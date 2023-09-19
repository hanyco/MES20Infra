using Contracts.ViewModels;

using Library.CodeGeneration.Models;
using Library.Interfaces;

namespace Contracts.Services;

public interface IModelConverterCodeService : IBusinessService
{
    Code GenerateCode(DtoViewModel src, string dstClassName, string methodName);
}