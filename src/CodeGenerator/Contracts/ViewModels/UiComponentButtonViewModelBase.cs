using HanyCo.Infra.Internals.Data.DataSources;

namespace Contracts.ViewModels;

public abstract class UiComponentButtonViewModelBase : UiComponentContentViewModelBase
{
    private string? _eventHandlerName;
    private Placement _placement;
    private string? _returnType;
    public string? EventHandlerName { get => this._eventHandlerName; set => this.SetProperty(ref this._eventHandlerName, value); }

    public Placement Placement { get => this._placement; set => this.SetProperty(ref this._placement, value); }

    public string ReturnType { get => this._returnType == "void" ? string.Empty : _returnType; set => this.SetProperty(ref this._returnType, value); }
}