namespace Contracts.ViewModels;

public sealed class UiComponentCustomButton : UiComponentButtonViewModelBase, IUiComponentCustomContent, FrontElement
{
    private string? _codeStatement;

    public string? CodeStatement { get => this._codeStatement; set => this.SetProperty(ref this._codeStatement, value); }
}