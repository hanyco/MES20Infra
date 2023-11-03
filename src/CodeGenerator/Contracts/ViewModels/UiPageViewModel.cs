#nullable disable

using System.Collections.ObjectModel;

using HanyCo.Infra.UI.ViewModels;

using Library.CodeGeneration;

namespace Contracts.ViewModels;

public sealed class UiPageViewModel : UiComponentViewModelBase
{
    private string _className;
    private DtoViewModel _dataContext;
    private bool _generateMainCode;
    private bool _generatePartialCode;
    private bool _generateUiCode;
    private ModuleViewModel _module;
    private string _nameSpace;

    public string ClassName { get => this._className; set => this.SetProperty(ref this._className, value); }
    public ObservableCollection<UiComponentViewModel> Components { get; } = [];
    public DtoViewModel DataContext { get => this._dataContext; set => this.SetProperty(ref this._dataContext, value); }
    public bool GenerateMainCode { get => this._generateMainCode; set => this.SetProperty(ref this._generateMainCode, value); }
    public bool GeneratePartialCode { get => this._generatePartialCode; set => this.SetProperty(ref this._generatePartialCode, value); }
    public bool GenerateUiCode { get => this._generateUiCode; set => this.SetProperty(ref this._generateUiCode, value); }
    public ModuleViewModel Module { get => this._module; set => this.SetProperty(ref this._module, value); }
    public string NameSpace { get => this._nameSpace; set => this.SetProperty(ref this._nameSpace, value); }
    public ObservableCollection<(TypePath Type, string Name)> Parameters { get; } = [];
    public ObservableCollection<string> Routes { get; } = [];
}