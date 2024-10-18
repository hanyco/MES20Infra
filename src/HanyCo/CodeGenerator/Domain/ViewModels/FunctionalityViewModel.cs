using System.Diagnostics.CodeAnalysis;

namespace HanyCo.Infra.CodeGen.Domain.ViewModels;

public sealed class FunctionalityViewModel : InfraViewModelBase
{
    private DtoViewModel _sourceDto;
    private ControllerViewModel controller = new();

    public UiComponentViewModel BlazorDetailsComponent { get; set; }

    public UiPageViewModel BlazorDetailsPage { get; set; }

    public UiComponentViewModel BlazorListComponent { get; set; }

    public UiPageViewModel BlazorListPage { get; set; }

    public FunctionalityViewModelCodes Codes { get; } = new();

    [NotNull]
    public ControllerViewModel Controller { get => this.controller ??= new(); set => this.controller = value; }

    public CqrsCommandViewModel DeleteCommand { get; set; }

    public CqrsQueryViewModel GetAllQuery { get; set; }

    public CqrsQueryViewModel GetByIdQuery { get; set; }

    public CqrsCommandViewModel InsertCommand { get; set; }

    public MapperGeneratorViewModel MapperGeneratorViewModel { get; } = new();

    public ModuleViewModel Module { get; set; }
    
    public DtoViewModel SourceDto { get => this._sourceDto; set => this.SetProperty(ref this._sourceDto, value); }

    public CqrsCommandViewModel UpdateCommand { get; set; }
}