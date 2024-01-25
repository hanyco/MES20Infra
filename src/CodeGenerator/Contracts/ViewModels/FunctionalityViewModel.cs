namespace HanyCo.Infra.CodeGen.Contracts.ViewModels;

/// <summary>
/// This class represents a functionality-specific view model. It inherits from InfraViewModelBase
/// and is designed to store various components and codes related to a specific functionality. This
/// class is closely related to the FunctionalityViewModelCodes class for storing
/// functionality-specific codes.
/// </summary>
public sealed class FunctionalityViewModel : InfraViewModelBase
{
    private DtoViewModel _sourceDto;

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

    public ApiCodingViewModel ApiCodingViewModel { get; set; } = new();
}