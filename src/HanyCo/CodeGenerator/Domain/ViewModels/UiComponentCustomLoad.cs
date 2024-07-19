namespace HanyCo.Infra.CodeGen.Contracts.ViewModels;

public sealed class UiComponentCustomLoad : InfraViewModelBase, IUiComponentCustomContent, BackElement
{
    private string? _codeStatement;

    public UiComponentCustomLoad()
        : base(null, name: "OnCustomLoad")
    {
    }

    public string? CodeStatement { get => this._codeStatement; set => this.SetProperty(ref this._codeStatement, value); }
}