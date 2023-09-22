#nullable disable

using System.Collections.ObjectModel;
using System.Windows.Input;

using Contracts.ViewModels;

using Library.Data.Markers;
using Library.Wpf.Windows.Input.Commands;

namespace HanyCo.Infra.UI.ViewModels;

public sealed class UiPageViewModel : UiComponentViewModelBase
{
    #region Fields

    private string _ClassName;
    private DtoViewModel _dataContext;
    private bool _GenerateMainCode;
    private bool _GeneratePartialCode;
    private bool _GenerateUiCode;
    private ModuleViewModel _Module;
    private string _NameSpace;
    private string _Route;

    #endregion

    public string ClassName { get => this._ClassName; set => this.SetProperty(ref this._ClassName, value); }

    public ObservableCollection<UiViewModel> Components { get; } = new();

    public DtoViewModel DataContext { get => this._dataContext; set => this.SetProperty(ref this._dataContext, value); }

    public bool GenerateMainCode { get => this._GenerateMainCode; set => this.SetProperty(ref this._GenerateMainCode, value); }

    public bool GeneratePartialCode { get => this._GeneratePartialCode; set => this.SetProperty(ref this._GeneratePartialCode, value); }

    public bool GenerateUiCode { get => this._GenerateUiCode; set => this.SetProperty(ref this._GenerateUiCode, value); }

    public ModuleViewModel Module { get => this._Module; set => this.SetProperty(ref this._Module, value); }

    public string NameSpace { get => this._NameSpace; set => this.SetProperty(ref this._NameSpace, value); }

    public string Route { get => this._Route; set => this.SetProperty(ref this._Route, value); }
}
