namespace Contracts.ViewModels;

public sealed class ModelConverterCodeViewModel : InfraViewModelBase
{
    public ModelConverterCodeViewModel()
    {
    }

    public ModelConverterCodeViewModel(long? id, string name, DtoViewModel sourceDto, string srcClassName, string dstClassName, string methodName)
        : base(id, name)
    {
        this.SourceDto = sourceDto;
        this.SrcClassName = srcClassName;
        this.DstClassName = dstClassName;
        this.MethodName = methodName;
    }

    public string DstClassName { get; }
    public string MethodName { get; }
    public DtoViewModel SourceDto { get; }
    public string SrcClassName { get; }

    public void Deconstruct(out DtoViewModel sourceDto, out string srcClassName, out string dstClassName, out string methodName) =>
        (sourceDto, srcClassName, dstClassName, methodName) = (this.SourceDto, this.SrcClassName, this.DstClassName, this.MethodName);
}