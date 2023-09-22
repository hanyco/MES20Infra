using HanyCo.Infra.Internals.Data.DataSources;

namespace Contracts.ViewModels;

public interface BackElement
{
}

public interface FrontElement
{
}

public interface IUiComponentContent
{
}

public interface IUiComponentCqrsContent : IUiComponentContent
{
    CqrsViewModelBase? CqrsSegregate { get; set; }
}

public abstract class UiComponentContentViewModelBase : InfraViewModelBase, IUiComponentContent
{
    private string? _caption;
    private bool _isEnabled = true;
    private UiBootstrapPositionViewModel? _position;

    public string Caption { get => this._caption ??= string.Empty; set => this.SetProperty(ref this._caption, value); }

    public bool IsEnabled { get => this._isEnabled; set => this.SetProperty(ref this._isEnabled, value); }

    public UiBootstrapPositionViewModel Position { get => this._position ??= new(); set => this._position = value; }
}

public abstract class UiComponentButtonViewModelBase : UiComponentContentViewModelBase
{
    private string? _eventHandlerName;
    private Placement _placement;

    public string? EventHandlerName { get => this._eventHandlerName; set => this.SetProperty(ref this._eventHandlerName, value); }

    public Placement Placement { get => this._placement; set => this.SetProperty(ref this._placement, value); }
}

public sealed class UiComponentCqrsButtonViewModel : UiComponentButtonViewModelBase, IUiComponentCqrsContent, FrontElement
{
    private CqrsViewModelBase? _cqrsSegregate;

    public CqrsViewModelBase? CqrsSegregate { get => this._cqrsSegregate; set => this.SetProperty(ref this._cqrsSegregate, value); }
}

public sealed class UiComponentCustomButtonViewModel : UiComponentButtonViewModelBase, FrontElement
{
    private FormattableString? _codeStatement;

    public FormattableString? CodeStatement { get => this._codeStatement; set => this.SetProperty(ref this._codeStatement, value); }
}

public sealed class UiComponentLoadViewModel : InfraViewModelBase, IUiComponentCqrsContent, BackElement
{
    private CqrsViewModelBase? _cqrsSegregate;

    public CqrsViewModelBase? CqrsSegregate { get => this._cqrsSegregate; set => this.SetProperty(ref this._cqrsSegregate, value); }
}

public sealed class UiPropertyViewModel : UiComponentContentViewModelBase, BackElement
{
    private UiComponentViewModel? _component;
    private ControlType? _controlType;
    private PropertyViewModel? _property;

    public UiComponentViewModel Component { get => this._component ??= new(); set => this.SetProperty(ref this._component, value); }

    public ControlType? ControlType { get => this._controlType; set => this.SetProperty(ref this._controlType, value); }

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