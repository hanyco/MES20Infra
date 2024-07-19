using Library.ComponentModel;

namespace HanyCo.Infra.UI.ViewModels;

public sealed class SettingsModel : NotifyPropertyChanged
{
    private string? _connectionString;
    private bool _showToast;

    public string? connectionString
    {
        get => this._connectionString;
        set => this.SetProperty(ref this._connectionString, value);
    }

    public bool showToast
    {
        get => this._showToast;
        set => this.SetProperty(ref this._showToast, value);
    }
}