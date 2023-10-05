using Contracts.ViewModels;

using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

using Library.CodeGeneration.Models;
using Library.Interfaces;
using Library.Results;

namespace Contracts.Services;

public interface IModelConverterCodeService : IBusinessService, ICodeGenerator<ModelConverterCodeViewModel>
{
}