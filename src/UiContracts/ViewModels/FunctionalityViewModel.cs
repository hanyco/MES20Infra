#nullable disable

using Contracts.ViewModels;

using Library.Data.SqlServer.Dynamics;

namespace HanyCo.Infra.UI.ViewModels;

public sealed class FunctionalityViewModel : InfraViewModelBase
{
    private long _moduleId;
    private string _nameSpace;
    //x private DtoViewModel _rootDto;
    private DbObjectViewModel _dbObject;

    public UiComponentViewModel BlazorDetailsComponent { get; set; }

    public UiComponentViewModel BlazorListComponent { get; set; }

    public CqrsCommandViewModel DeleteCommand { get; set; }

    public DtoViewModel DetailsViewModel { get; set; }

    public CqrsQueryViewModel GetAllQueryViewModel { get; set; }

    public CqrsQueryViewModel GetByIdQueryViewModel { get; set; }

    public CqrsCommandViewModel InsertCommand { get; set; }

    public DtoViewModel ListViewModel { get; set; }

    public long ModuleId { get => this._moduleId; set => this.SetProperty(ref this._moduleId, value); }

    public string NameSpace { get => this._nameSpace; set => this.SetProperty(ref this._nameSpace, value); }

    //x public DtoViewModel RootDto { get => this._rootDto; set => this.SetProperty(ref this._rootDto, value); }
    public DbObjectViewModel DbObject { get => this._dbObject; set => this.SetProperty(ref this._dbObject, value); }

    public Table DbTable { get; set; } = null;

    public CqrsCommandViewModel UpdateCommand { get; set; }
}