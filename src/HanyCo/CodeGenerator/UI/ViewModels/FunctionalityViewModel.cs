#nullable disable

namespace HanyCo.Infra.UI.ViewModels;

public sealed class FunctionalityViewModel : InfraViewModelBase
{
    private long _moduleId;
    private string _nameSpace;
    private DtoViewModel _rootDto;

    public UiComponentViewModel BlazorDetailsComponent { get; internal set; }

    public UiComponentViewModel BlazorListComponent { get; internal set; }

    public CqrsCommandViewModel DeleteCommand { get; internal set; }

    public DtoViewModel DetailsViewModel { get; internal set; }

    public CqrsQueryViewModel GetAllQueryViewModel { get; internal set; }

    public CqrsQueryViewModel GetByIdQueryViewModel { get; internal set; }

    public CqrsCommandViewModel InsertCommand { get; internal set; }

    public DtoViewModel ListViewModel { get; internal set; }

    public long ModuleId { get => this._moduleId; set => this.SetProperty(ref this._moduleId, value); }

    public string NameSpace { get => this._nameSpace; set => this.SetProperty(ref this._nameSpace, value); }

    public DtoViewModel RootDto { get => this._rootDto; set => this.SetProperty(ref this._rootDto, value); }

    public CqrsCommandViewModel UpdateCommand { get; internal set; }
}