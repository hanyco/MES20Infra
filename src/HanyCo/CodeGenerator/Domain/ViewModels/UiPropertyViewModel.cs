using HanyCo.Infra.Internals.Data.DataSources;

namespace HanyCo.Infra.CodeGen.Domain.ViewModels;

public sealed class UiPropertyViewModel : UiComponentContentViewModelBase, FrontElement
{
    private UiComponentViewModel? _component;
    private ControlType? _controlType;
    private bool _isParameter;
    private PropertyViewModel? _property;

    public UiComponentViewModel Component { get => this._component ??= new(); set => this.SetProperty(ref this._component, value); }

    public ControlType? ControlType { get => this._controlType; set => this.SetProperty(ref this._controlType, value); }

    public bool IsParameter { get => this._isParameter; set => this.SetProperty(ref this._isParameter, value); }
    public override string? Name => this.Property?.Name;

    public PropertyViewModel? Property
    {
        get => this._property;

        set
        {
            this.SetProperty(ref this._property, value);
            this.OnPropertyChanged(nameof(this.Name));
        }
    }
}