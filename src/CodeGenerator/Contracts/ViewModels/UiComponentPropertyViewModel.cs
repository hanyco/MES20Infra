#nullable disable

using Contracts.ViewModels;

using HanyCo.Infra.Internals.Data.DataSources;

namespace HanyCo.Infra.UI.ViewModels;

public sealed class UiComponentPropertyViewModel : UiComponentContentViewModelBase
{
    #region Fields

    private UiComponentViewModel _Component;
    private ControlType? _ControlType;
    private PropertyViewModel _Property;

    #endregion

    public UiComponentViewModel Component { get => this._Component; set => this.SetProperty(ref this._Component, value); }

    public ControlType? ControlType { get => this._ControlType; set => this.SetProperty(ref this._ControlType, value); }

    public PropertyViewModel Property
    {
        get => this._Property; set
        {
            this.SetProperty(ref this._Property, value);
            this.OnPropertyChanged(nameof(this.Name));
        }
    }

    public string ControlExtraInfo { get; set; }

    public override string Name => this.Property?.Name;
}
