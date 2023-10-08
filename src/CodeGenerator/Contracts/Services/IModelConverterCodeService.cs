using Contracts.ViewModels;

using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

using Library.CodeGeneration;
using Library.Interfaces;

namespace Contracts.Services;

public interface IModelConverterCodeService : IBusinessService, ICodeGenerator<ModelConverterCodeParameter>
{
}

public sealed class ModelConverterCodeParameter(DtoViewModel sourceDto, string srcClass, string dstClass, string? methodName)
{
    public TypePath DstClass { get; } = dstClass;
    public string? MethodName { get; } = methodName;
    public DtoViewModel SourceDto { get; } = sourceDto;
    public TypePath SrcClass { get; } = srcClass;

    public void Deconstruct(out DtoViewModel sourceDto, out TypePath srcClass, out TypePath dstClass, out string methodName) =>
        (sourceDto, srcClass, dstClass, methodName) = (this.SourceDto, this.SrcClass, this.DstClass, this.MethodName);
}