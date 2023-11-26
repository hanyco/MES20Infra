namespace Contracts.ViewModels;

public sealed class UiComponentCustomLoadViewModel : InfraViewModelBase, IUiComponentCustomContent, BackElement
{
    private string? _codeStatement;

    public UiComponentCustomLoadViewModel()
        : base(null, name: "OnCustomLoad")
    {
    }

    public string? CodeStatement { get => this._codeStatement; set => this.SetProperty(ref this._codeStatement, value); }
}
