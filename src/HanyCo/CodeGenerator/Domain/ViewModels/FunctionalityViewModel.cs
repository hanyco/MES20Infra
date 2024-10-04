using HanyCo.Infra.CodeGen.Contracts.CodeGen.ViewModels;
using HanyCo.Infra.CodeGen.Contracts.ViewModels;

namespace HanyCo.Infra.CodeGen.Domain.ViewModels;

public sealed class FunctionalityViewModel : InfraViewModelBase
{
    private DtoViewModel _sourceDto;

    public ApiCodingViewModel ApiCodingViewModel { get; set; } = new();

    public UiComponentViewModel BlazorDetailsComponent { get; set; }

    public UiPageViewModel BlazorDetailsPage { get; set; }

    public UiComponentViewModel BlazorListComponent { get; set; }

    public UiPageViewModel BlazorListPage { get; set; }

    public FunctionalityViewModelCodes Codes { get; } = new();

    public CqrsCommandViewModel DeleteCommand { get; set; }

    public CqrsQueryViewModel GetAllQuery { get; set; }

    public CqrsQueryViewModel GetByIdQuery { get; set; }

    public CqrsCommandViewModel InsertCommand { get; set; }

    public MapperGeneratorViewModel MapperGeneratorViewModel { get; } = new();

    public DtoViewModel SourceDto { get => this._sourceDto; set => this.SetProperty(ref this._sourceDto, value); }

    public CqrsCommandViewModel UpdateCommand { get; set; }
}