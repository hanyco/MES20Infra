using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows;

using Contracts.Services;
using Contracts.ViewModels;

using HanyCo.Infra.UI.UserControls;
using HanyCo.Infra.UI.ViewModels;

using Library.BusinessServices;
using Library.CodeGeneration.Models;
using Library.EventsArgs;
using Library.Exceptions;
using Library.Results;
using Library.Threading.MultistepProgress;
using Library.Validations;
using Library.Wpf.Dialogs;
using Library.Wpf.Windows;

using Microsoft.WindowsAPICodePack.Dialogs;

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

    public async Task<Result> SaveDbAsync()
    {
        this.CheckIfInitiated();
        this.PrepareViewModel();
        if (!ResultHelper.TryParse(await this.ValidateFormAsync(), out var validationResult))
        {
            return validationResult;
        }

        var result = await this._service.SaveViewModelAsync(this.ViewModel);
        if (result)
        {
            _ = this.SetIsViewModelChanged(false);
        }

        return result;
    }

    protected override async Task OnBindDataAsync()
    {
        await this.FunctionalityTreeView.BindAsync();
        await base.OnBindDataAsync();
    }

    [MemberNotNull(nameof(ViewModel))]
    private void CheckIfInitiated() =>
        this.ViewModel.NotNull(() => "Please create a new Functionality or edit an old one.");

    private async void CreateFunctionalityButton_Click(object sender, RoutedEventArgs e)
    {
        _ = await this.AskToSaveIfChangedAsync().BreakOnFail();
        this.ViewModel = await this._service.CreateAsync();
    }

    private async void DeleteFunctionalityButton_Click(object sender, RoutedEventArgs e)
    {
        Check.MustBeNotNull(this.FunctionalityTreeView.SelectedItem, () => new CommonException("No functionality selected.", "Please select functionality", details: "If there is not functionality, please create one"));
        var resp = MsgBox2.AskWithWarn("Are you sure you want to delete this Functionality?", "This operation cannot be undone.", detailsExpandedText: "Any DTO, View Model and CQRS segregation associated to this Functionality will be deleted.");
        _ = Check.If(resp != TaskDialogResult.Ok).BreakOnFail();
        _ = await this._service.DeleteAsync(this.FunctionalityTreeView.SelectedItem).ShowOrThrowAsync(this.Title);
    }

    private async void GenerateCodesButton_Click(object sender, RoutedEventArgs e)
    {
        _ = await this.ValidateFormAsync().ThrowOnFailAsync(this.Title);

        (_, var code) = await this.ActionScopeRunAsync(() => this._codeService.GenerateCodesAsync(this.ViewModel!, new(true)), "Generating codes...").ShowOrThrowAsync(this.Title);
        this.ComponentCodeResultUserControl.Codes = code;
    }

    private async void GenerateViewModelButton_Click(object sender, RoutedEventArgs e)
    {
        _ = await this.ValidateFormAsync().ThrowOnFailAsync(this.Title);
        this.PrepareViewModel();
        using var scope = this.ActionScopeBegin("Creating view models...");
        (_, var viewModel) = await this._service.GenerateViewModelAsync(this.ViewModel).ShowOrThrowAsync(this.Title);
        this.ViewModel = viewModel;
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

    [MemberNotNull(nameof(ViewModel))]
    private void PrepareViewModel()
    {
        this.ViewModel!.Name ??= this.ViewModel.SourceDto.Name;
        this.ViewModel.NameSpace ??= this.ViewModel.SourceDto.NameSpace;
    }

    private void PrepareViewModelByDto(DtoViewModel? details)
    {
        this.CheckIfInitiated();

        this.ViewModel.SourceDto = null;

        //Optional! To make sure that the selected dto exists and has details.
        if (details == null)
        {
            return;
        }

        this.ViewModel.SourceDto = details;
        //Maybe the user filled these data. We shouldn't overwrite user's preferences. If user
        // presses <Reset> button, user's preferences will be cleaned.
        if (this.ViewModel.NameSpace.IsNullOrEmpty())
        {
            this.ViewModel.NameSpace = details.NameSpace;
        }

        if (this.ViewModel.Name.IsNullOrEmpty())
        {
            this.ViewModel.Name = details.Name;
        }
        //The form is now ready to call services.
    }

    private async Task<Result> SaveCodes()
    {
        if (!ResultHelper.TryParse(await this.ValidateFormAsync(), out var validationResult))
        {
            return validationResult;
        };
        try
        {
            var codes = this.ViewModel!.Codes.SelectAll().Compact();
            if (!codes.Any())
            {
                return Result.CreateFailure("No code found. Please generate sources.");
            }
            var settings = SettingsService.Get();
            foreach (var code in codes)
            {
                var relativePath = code.props().Category switch
                {
                    CodeCategory.Dto => settings.dtosPath,
                    CodeCategory.Query => settings.queriesPath,
                    CodeCategory.Command => settings.commandsPath,
                    CodeCategory.Page => settings.blazorPagesPath,
                    CodeCategory.Component => settings.blazorComponentsPath,
                    _ => throw new NotSupportedException("Code category is null or not supported.")
                };
                var path = Path.Combine(settings.projectSourceRoot.NotNull(), relativePath.NotNull());
                if (!Directory.Exists(path))
                {
                    _ = Directory.CreateDirectory(path);
                }
                File.WriteAllText(Path.Combine(path, code.FileName), code.Statement);
            }
            return Result.Success;
        }
        catch (Exception ex)
        {
            return Result.CreateFailure(ex);
        }
    }

    private async void SaveToDbButton_Click(object sender, RoutedEventArgs e) =>
        await this.SaveDbAsync().ShowOrThrowAsync(this.Title);

    private async void SaveToDiskButton_Click(object sender, RoutedEventArgs e) =>
        await this.SaveCodes().ShowOrThrowAsync(this.Title);

    private async void SelectRootDtoByDtoButton_Click(object sender, RoutedEventArgs e)
    {
        this.SelectRootDtoByDtoButton.IsEnabled = false;

        try
        {
            this.CheckIfInitiated();
            _ = await this.AskToSaveIfChangedAsync().BreakOnFail();

            //Let user to select a DTO
            this._dtoExplorerTreeView ??= new CqrsExplorerTreeView { LoadDtos = true };
            _ = HostDialog.ShowDialog(this._dtoExplorerTreeView, "Select Root DTO", "Select a DTO to create a Functionality.", _ => Check.If(this._dtoExplorerTreeView.SelectedItem is not DtoViewModel, () => "Please select a DTO.")).BreakOnFail();

            //Did user select a DTO?
            if (this._dtoExplorerTreeView.SelectedItem is DtoViewModel dto)
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
            _ = HostDialog.ShowDialog(this._databaseExplorerUserControl, "Select Root Table", "Select a table to create a Functionality.", _ => Check.If(this._databaseExplorerUserControl.SelectedTable is null, () => "Please select a table.")).BreakOnFail();

            // Did user select a DTO?
            var table = this._databaseExplorerUserControl.SelectedTable!;
            var columns = await this._dbTableService.GetColumnsAsync(SettingsService.Get().connectionString!, table.Name!);
            var dto = this._dtoService.CreateByDbTable(table, columns);
            this.PrepareViewModelByDto(dto);
        }
        finally
        {
            this.SelectRootDtoByTableButton.IsEnabled = true;
        }
    }
}