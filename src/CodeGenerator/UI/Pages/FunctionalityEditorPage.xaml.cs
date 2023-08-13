using System.Diagnostics.CodeAnalysis;
using System.Windows;

using Contracts.Services;
using Contracts.ViewModels;

using HanyCo.Infra.UI.Services.Imp;
using HanyCo.Infra.UI.UserControls;
using HanyCo.Infra.UI.ViewModels;

using Library.EventsArgs;
using Library.Results;
using Library.Threading.MultistepProgress;
using Library.Validations;
using Library.Wpf.Dialogs;
using Library.Wpf.Windows;
using Library.Wpf.Windows.UI;

namespace HanyCo.Infra.UI.Pages;

/// <summary>
/// Interaction logic for FunctionalityEditorPage.xaml
/// </summary>
public partial class FunctionalityEditorPage : IStatefulPage, IAsyncSavePage
{
    private readonly IFunctionalityCodeService _codeService;
    private readonly IEntityViewModelConverter _converter;
    private readonly IDbTableService _dbTableService;
    private readonly IDtoService _dtoService;
    private readonly IModuleService _moduleService;
    private readonly IPropertyService _propertyService;
    private readonly IProgressReport _reporter;
    private readonly IFunctionalityService _service;
    private DatabaseExplorerUserControl? _databaseExplorerUserControl;
    private CqrsExplorerTreeView? _dtoExplorerTreeView;

    public FunctionalityEditorPage(
        IFunctionalityService service,
        IFunctionalityCodeService codeService,
        IModuleService moduleService,
        IDbTableService dbTableService,
        IProgressReport reporter,
        IDtoService dtoService,
        IPropertyService propertyService,
        IEntityViewModelConverter converter,
        ILogger logger)
        : base(logger)
    {
        this._service = service;
        this._codeService = codeService;
        this._moduleService = moduleService;
        this._dbTableService = dbTableService;
        this._reporter = reporter;
        this._dtoService = dtoService;
        this._propertyService = propertyService;
        this._converter = converter;
        this.InitializeComponent();
    }

    bool IStatefulPage.IsViewModelChanged { get; set; }

    public FunctionalityViewModel? ViewModel
    {
        get => this.GetViewModelByDataContext<FunctionalityViewModel>();
        set => this.SetViewModelByDataContext(value, () => this.DtoViewModelEditor.IsEnabled = this.ViewModel?.SourceDto != null);
    }

    public async Task<Result<int>> SaveAsync()
    {
        this.CheckIfInitiated();
        if (!this.GetIsViewModelChanged())
        {
            return Result<int>.CreateSuccess(0);
        }

        _ = await this.ValidateFormAsync().ThrowOnFailAsync(this.Title);

        var result = await this._service.SaveViewModelAsync(this.ViewModel!).ThrowOnFailAsync(this.Title);
        _ = this.SetIsViewModelChanged(false);
        return result.WithValue(0);
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
        _ = await this.AskToSaveIfChangedAsync().BreakOnFail();
        this.ViewModel = new();
    }

    private async void GenerateCodesButton_Click(object sender, RoutedEventArgs e)
    {
        _ = await this.ValidateFormAsync().ThrowOnFailAsync(this.Title);
        var code = await this.ActionScopeRunAsync(() => this._codeService.GenerateCodesAsync(this.ViewModel!, new(true)), "Generating codes...").ThrowOnFailAsync(this.Title);
        if (!code.Message.IsNullOrEmpty())
        {
            Toast2.ShowText(code.Message);
            this.Logger.Debug(code.Message);
        }
        this.ComponentCodeResultUserControl.Codes = code!;
    }

    private async void GenerateViewModelButton_Click(object sender, RoutedEventArgs e)
    {
        _ = await this.ValidateFormAsync().ThrowOnFailAsync(this.Title);

        this.ViewModel!.Name ??= this.ViewModel.SourceDto.Name;
        this.ViewModel!.NameSpace ??= this.ViewModel.SourceDto.NameSpace;
        var scope = this.ActionScopeBegin("Creating view models...");
        var viewModel = await this._service.GenerateViewModelAsync(this.ViewModel).WithAsync(x => scope.End(x)).ThrowOnFailAsync(this.Title);
        if (!viewModel.Message.IsNullOrEmpty())
        {
            Toast2.New().AddText(viewModel.Message).Show();
            this.Logger.Debug(viewModel.Message);
        }
        this.ViewModel = viewModel!;
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

    private async void SaveToDbButton_Click(object sender, RoutedEventArgs e) => 
        await this.SaveAsync();

    private async void SelectRootDtoByDtoButton_Click(object sender, RoutedEventArgs e)
    {
        this.SelectRootDtoByDtoButton.IsEnabled = false;

        try
        {
            this.CheckIfInitiated();
            _ = await this.AskToSaveIfChangedAsync().BreakOnFail();

            //Let user to select a DTO
            this._dtoExplorerTreeView ??= new CqrsExplorerTreeView { LoadDtos = true };
            var isSelected = HostDialog.ShowDialog(this._dtoExplorerTreeView, "Select Root DTO", "Select a DTO to create a Functionality.", _ => Check.If(this._dtoExplorerTreeView.SelectedItem is not DtoViewModel, () => "Please select a DTO."));
            //Did user select a DTO?
            if (isSelected && this._dtoExplorerTreeView.SelectedItem is DtoViewModel dto)
            {
                var details = await this._dtoService.GetByIdAsync(dto.Id!.Value);
                this.PrepareViewModelByDto(details);
            }
        }
        finally
        {
            this.SelectRootDtoByDtoButton.IsEnabled = true;
        }
    }

    private async void SelectRootDtoByTableButton_Click(object sender, RoutedEventArgs e)
    {
        this.SelectRootDtoByTableButton.IsEnabled = false;

        try
        {
            this.CheckIfInitiated();
            _ = await this.AskToSaveIfChangedAsync().BreakOnFail();

            // Let user to select a table
            if (this._databaseExplorerUserControl == null)
            {
                this._databaseExplorerUserControl = new DatabaseExplorerUserControl();
                _ = await this._databaseExplorerUserControl.InitializeAsync(this._dbTableService, this._reporter);
            }
            var isSelected = HostDialog.ShowDialog(this._databaseExplorerUserControl, "Select Root Table", "Select a table to create a Functionality.", _ => Check.If(this._databaseExplorerUserControl.SelectedTable is null, () => "Please select a table."));
            //Did user select a DTO?
            var table = this._databaseExplorerUserControl.SelectedTable;
            if (isSelected && table is not null)
            {
                var columns = await this._dbTableService.GetColumnsAsync(SettingsService.Get().connectionString!, table.Name!);
                var dto = this._dtoService.CreateByDbTable(table, columns);
                this.PrepareViewModelByDto(dto);
            }
        }
        finally
        {
            this.SelectRootDtoByTableButton.IsEnabled = true;
        }
    }
}