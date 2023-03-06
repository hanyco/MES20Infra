namespace HanyCo.Infra.UI.ViewModels;

public abstract class UiComponentContentViewModelBase : InfraViewModelBase
{
    #region Fields

    private string _caption;
    private bool _isEnabled;
    private UiBootstrapPositionViewModel _position = new();

    #endregion Fields

    public string Caption { get => this._caption ?? string.Empty; set => this.SetProperty(ref this._caption, value); }

    public bool IsEnabled { get => this._isEnabled; set => this.SetProperty(ref this._isEnabled, value); }

    public UiBootstrapPositionViewModel Position { get => _position; set => _position = value ?? new(); }
}