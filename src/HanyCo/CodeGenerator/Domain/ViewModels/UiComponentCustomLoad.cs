namespace HanyCo.Infra.CodeGen.Domain.ViewModels;

public sealed class UiComponentCustomLoad : InfraViewModelBase, IUiComponentCustomContent, BackElement
{
    public UiComponentCustomLoad()
        : base(null, name: "OnCustomLoad")
    {
    }

    public string? CodeStatement { get; set => this.SetProperty(ref field, value); }
}