using Library.ComponentModel;

namespace HanyCo.Infra.CodeGen.Contracts.ViewModels;

public sealed class SettingsModel : NotifyPropertyChanged
{
    private string? _blazorComponentsPath;
    private string? _blazorPagesPath;
    private string? _commandsPath;
    private string? _connectionString;
    private string? _dtosPath;
    private string? _convertersPath;
    private string? _productName;
    private string? _projectSourceRootPath;
    private string? _queriesPath;
    private bool _showToast;

    public string? blazorComponentsPath
    {
        get => this._blazorComponentsPath;
        set => this.SetProperty(ref this._blazorComponentsPath, value);
    }

    public string? blazorPagesPath
    {
        get => this._blazorPagesPath;
        set => this.SetProperty(ref this._blazorPagesPath, value);
    }

    public string? commandsPath
    {
        get => this._commandsPath;
        set => this.SetProperty(ref this._commandsPath, value);
    }

    public string? connectionString
    {
        get => this._connectionString;
        set => this.SetProperty(ref this._connectionString, value);
    }

    public string? dtosPath
    {
        get => this._dtosPath;
        set => this.SetProperty(ref this._dtosPath, value);
    }

    public string? convertersPath
    {
        get => this._convertersPath;
        set => this.SetProperty(ref this._convertersPath, value);
    }

    public string? productName
    {
        get => this._productName;
        set => this.SetProperty(ref this._productName, value);
    }

    public string? projectSourceRoot
    {
        get => this._projectSourceRootPath;
        set => this.SetProperty(ref this._projectSourceRootPath, value);
    }

    public string? queriesPath
    {
        get => this._queriesPath;
        set => this.SetProperty(ref this._queriesPath, value);
    }

    public bool showToast
    {
        get => this._showToast;
        set => this.SetProperty(ref this._showToast, value);
    }
}