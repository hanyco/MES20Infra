using Contracts.ViewModels;

using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

using Library.CodeGeneration;
using Library.Interfaces;

namespace Contracts.Services;

public interface IModelConverterCodeService : IBusinessService, ICodeGenerator<ModelConverterCodeParameter>
{
}

public readonly record struct ModelConverterCodeParameter(
    DtoViewModel SourceDto,
    string SrcClass,
    TypePath DstClass,
    bool IsSrcEnumerable);