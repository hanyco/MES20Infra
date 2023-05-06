#nullable disable

using System.Collections;
using System.Runtime.CompilerServices;

using HanyCo.Infra.UI.ViewModels;

using Library.CodeGeneration.Models;
using Library.Data.SqlServer.Dynamics;

namespace Contracts.ViewModels;

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

    public CqrsQueryViewModel GetAllQuery { get; set; }

    public CqrsQueryViewModel GetByIdQuery { get; set; }

    public CqrsCommandViewModel InsertCommand { get; set; }

    public DtoViewModel ListViewModel { get; set; }

    public long ModuleId { get => this._moduleId; set => this.SetProperty(ref this._moduleId, value); }

    public string NameSpace { get => this._nameSpace; set => this.SetProperty(ref this._nameSpace, value); }

    //x public DtoViewModel RootDto { get => this._rootDto; set => this.SetProperty(ref this._rootDto, value); }
    public DbObjectViewModel DbObject { get => this._dbObject; set => this.SetProperty(ref this._dbObject, value); }

    public Table DbTable { get; set; } = null;

    public CqrsCommandViewModel UpdateCommand { get; set; }

    public FunctionalityViewModelCodes Codes { get; } = new();
}

public sealed class FunctionalityViewModelCodes
{
    private readonly Dictionary<string, Codes> _allCodes = new();

    public Codes GetAllQueryCodes { get => this.get(); set => this.set(value); }

    private void set(Codes value, [CallerMemberName] string propName = null)
        => this._allCodes[propName] = value;

    private Codes get([CallerMemberName] string propName = null)
        => this._allCodes[propName];
}