using System.Diagnostics.CodeAnalysis;
using System.Windows;

using Contracts.Services;
using Contracts.ViewModels;

using HanyCo.Infra.UI.Services;
using HanyCo.Infra.UI.UserControls;
using HanyCo.Infra.UI.ViewModels;

using Library.EventsArgs;
using Library.Results;
using Library.Threading.MultistepProgress;
using Library.Validations;
using Library.Wpf.Dialogs;
using Library.Wpf.Windows;

namespace HanyCo.Infra.UI.Pages;

/// <summary>
/// Interaction logic for FunctionalityEditorPage.xaml
/// </summary>
public partial class FunctionalityEditorPage : IStatefulPage, IAsyncSavePage
{
    private readonly IFunctionalityCodeService _codeService;
    private readonly IDtoService _dtoService;
    private readonly IModuleService _moduleService;
    private readonly IFunctionalityService _service;

    // This field should be created bcuz creating takes a long time. Used as a cash.
    private CqrsExplorerTreeView? _dtoExplorerTreeView;

    public FunctionalityEditorPage(
        IFunctionalityService service,
        IFunctionalityCodeService codeService,
        IModuleService moduleService,
        IDbTableService dbTableService,
        IProgressReport reporter,
        IDtoService dtoService,
        ILogger logger)
        : base(logger)
    {
        this.InitializeComponent();
        this._service = service;
        this._codeService = codeService;
        this._moduleService = moduleService;
        this._dtoService = dtoService;
    }

    bool IStatefulPage.IsViewModelChanged { get; set; }

    public FunctionalityViewModel? ViewModel
    {
        get => this.GetViewModelByDataContext<FunctionalityViewModel>();
        set => this.SetViewModelByDataContext(value, () => this.DtoViewModelEditor.IsEnabled = this.ViewModel?.SourceDto != null);
    }

    public async Task<Result<int>> SaveAsync()
    {
        _ = await this.ValidateFormAsync().ThrowOnFailAsync(this.Title);

        _ = await this._service.SaveViewModelAsync(this.ViewModel!).ThrowOnFailAsync(this.Title);
        _ = this.SetIsViewModelChanged(false);
        return Result<int>.CreateSuccess(0);
    }

    protected override Task<Result> OnValidateFormAsync()
    {
        this.CheckIfInitiated();
        return base.OnValidateFormAsync();
    }

    [MemberNotNull(nameof(ViewModel))]
    private void CheckIfInitiated() =>
        this.ViewModel.NotNull(() => "Please create a new Functionality or edit an old one.");

    private async void CreateFunctionalityButton_Click(object sender, RoutedEventArgs e)
    {
        _ = await this.AskToSaveAsync().BreakOnFail();
        this.ViewModel = new();
    }

    private async void GenerateCodesButton_Click(object sender, RoutedEventArgs e)
    {
        _ = await this.ValidateFormAsync().ThrowOnFailAsync(this.Title);
        this.ViewModel!.Name ??= this.ViewModel.SourceDto.Name;
        this.ViewModel.NameSpace ??= this.ViewModel.SourceDto.NameSpace;

        var scope = this.BeginActionScope("Generating...");
        this.ViewModel = await this._service.GenerateViewModelAsync(this.ViewModel!)
                                            .WithAsync(x => scope.End(x))
                                            .ThrowOnFailAsync(this.Title);
    }

    private void Me_Loaded(object sender, RoutedEventArgs e) =>
        this.ViewModel = new();

    private void ModuleComboBox_Initializing(object sender, InitialItemEventArgs<IModuleService> e) =>
        e.Item = this._moduleService;

    private void ModuleComboBox_SelectedModuleChanged(object sender, ItemActedEventArgs<ModuleViewModel> e)
    {
        this.CheckIfInitiated();

        if (e.Item is { } module)
        {
            this.ViewModel.SourceDto.NotNull().Module = module;
        }
    }

    private void PrepareViewModelByDto(DtoViewModel? details)
    {
        this.CheckIfInitiated();

        //Optional! To make sure that the selected dto exists and has details.
        if (details == null)
        {
            return;
        }

        this.ViewModel.SourceDto = null;
        this.ViewModel.SourceDto = details;
        //May be the user filled these data. We shouldn't overwrite user's preferences. If user
        // presses <Reset> button, user's preferences will be cleaned.
        if (this.ViewModel.NameSpace.IsNullOrEmpty())
        {
            this.ViewModel.NameSpace = details.NameSpace;
        }

        if (this.ViewModel.Name.IsNullOrEmpty())
        {
            this.ViewModel.Name = details.Name;
        }
        //The form is now ready to call service.
    }

    private async void SelectRootDtoButton_Click(object sender, RoutedEventArgs e)
    {
        this.CheckIfInitiated();

        _ = await this.AskToSaveAsync().BreakOnFail();

        //Let user to select a DTO
        this._dtoExplorerTreeView ??= new CqrsExplorerTreeView { LoadDtos = true };
        var isSelected = HostDialog.ShowDialog(this._dtoExplorerTreeView, "Select Root DTO", "Select a DTO to create a Functionality.", _ => Check.MustBe(this._dtoExplorerTreeView.SelectedItem is DtoViewModel, () => "Please select a DTO."));
        //Did user select a DTO?
        if (!isSelected || this._dtoExplorerTreeView.SelectedItem is not DtoViewModel dto) // I don like this. Not OOP.
        {
            return;
        }

        var details = await this._dtoService.GetByIdAsync(dto.Id!.Value);
        this.PrepareViewModelByDto(details);
    }
}