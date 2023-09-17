using System.Collections.ObjectModel;

using HanyCo.Infra.UI.ViewModels;

namespace Contracts.ViewModels;

public class UiComponentViewModel : UiComponentViewModelBase
{
    private string _className = null!;
    private bool _generateMainCode = true;
    private bool _generatePartialCode = true;
    private bool _generateUiCode = true;
    private bool _isGrid;
    private string? _nameSpace;
    private DtoViewModel? _pageDataContext = null;
    private PropertyViewModel? _pageDataContextProperty;

    public string ClassName { get => this._className; set => this.SetProperty(ref this._className, value); }

    public bool GenerateMainCode { get => this._generateMainCode; set => this.SetProperty(ref this._generateMainCode, value); }

    public bool GeneratePartialCode { get => this._generatePartialCode; set => this.SetProperty(ref this._generatePartialCode, value); }

    public bool GenerateUiCode { get => this._generateUiCode; set => this.SetProperty(ref this._generateUiCode, value); }

    public bool IsGrid { get => this._isGrid; set => this.SetProperty(ref this._isGrid, value); }

    public string? NameSpace { get => this._nameSpace; set => this.SetProperty(ref this._nameSpace, value); }

    public long? PageComponentId { get; set; }

    public DtoViewModel? PageDataContext { get => this._pageDataContext; set => this.SetProperty(ref this._pageDataContext, value); }

    public PropertyViewModel? PageDataContextProperty { get => this._pageDataContextProperty; set => this.SetProperty(ref this._pageDataContextProperty, value); }

    public string PageDataContextType => this.PageDataContext?.FullName ?? string.Empty;

    public UiBootstrapPositionViewModel Position { get; private set; } = new();

    //public ObservableCollection<UiComponentButtonViewModel> UiActions { get; } = new();
    public ObservableCollection<IUiComponentContent> UiActions { get; } = new();

    public long? UiPageComponentId { get; set; }

    public ObservableCollection<UiPropertyViewModel> UiProperties { get; } = new();
}