using Contracts.ViewModels;

using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

using Library.Interfaces;

namespace Contracts.Services;

public interface IModelConverterCodeService : IBusinessService, ICodeGenerator<ModelConverterCodeParameter>
{
}

public sealed class ModelConverterCodeParameter(DtoViewModel sourceDto, string srcClassName, string dstClassName, string methodName)
{
    public string DstClassName { get; } = dstClassName;
    public string MethodName { get; } = methodName;
    public DtoViewModel SourceDto { get; } = sourceDto;
    public string SrcClassName { get; } = srcClassName;

    public void Deconstruct(out DtoViewModel sourceDto, out string srcClassName, out string dstClassName, out string methodName) =>
        (sourceDto, srcClassName, dstClassName, methodName) = (this.SourceDto, this.SrcClassName, this.DstClassName, this.MethodName);
}