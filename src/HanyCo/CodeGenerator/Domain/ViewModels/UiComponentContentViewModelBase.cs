namespace HanyCo.Infra.CodeGen.Contracts.ViewModels;

public abstract class UiComponentContentViewModelBase : InfraViewModelBase, IUiComponentContent
{
    private string? _caption;
    private bool _isEnabled = true;
    private UiBootstrapPositionViewModel? _position;

    public string? Caption { get => this._caption ??= string.Empty; set => this.SetProperty(ref this._caption, value); }

    public bool IsEnabled { get => this._isEnabled; set => this.SetProperty(ref this._isEnabled, value); }

    public UiBootstrapPositionViewModel Position { get => this._position ??= new(); set => this.SetProperty(ref this._position, value); }
}