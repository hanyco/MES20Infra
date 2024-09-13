using HanyCo.Infra.CodeGen.Contracts.CodeGen.ViewModels;
using HanyCo.Infra.Internals.Data.DataSources;

namespace HanyCo.Infra.CodeGen.Contracts.ViewModels;

public sealed class FunctionalityViewModel : InfraViewModelBase
{
    private DtoViewModel _sourceDto;

    public ApiCodingViewModel ApiCodingViewModel { get; set; } = new();

    public UiComponentViewModel BlazorDetailsComponentViewModel { get; set; }

    public UiPageViewModel BlazorDetailsPageViewModel { get; set; }

    public UiComponentViewModel BlazorListComponentViewModel { get; set; }

    public UiPageViewModel BlazorListPageViewModel { get; set; }

    public FunctionalityViewModelCodes Codes { get; } = new();

    public CqrsCommandViewModel DeleteCommandViewModel { get; set; }

    public CqrsQueryViewModel GetAllQueryViewModel { get; set; }

    public CqrsQueryViewModel GetByIdQueryViewModel { get; set; }

    public CqrsCommandViewModel InsertCommandViewModel { get; set; }

    public MapperGeneratorViewModel MapperGeneratorViewModel { get; } = new();

    public DtoViewModel SourceDto { get => this._sourceDto; set => this.SetProperty(ref this._sourceDto, value); }

    public CqrsCommandViewModel UpdateCommandViewModel { get; set; }
}