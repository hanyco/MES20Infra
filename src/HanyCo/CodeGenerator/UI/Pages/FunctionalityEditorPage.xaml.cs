using HanyCo.Infra.CodeGen.Domain.Services;
using HanyCo.Infra.CodeGen.Domain.ViewModels;
using HanyCo.Infra.CodeGeneration.Definitions;
using HanyCo.Infra.UI.Helpers;
using HanyCo.Infra.UI.UserControls;

using Library.CodeGeneration.Models;
using Library.ComponentModel;
using Library.Data.EntityFrameworkCore;
using Library.EventsArgs;
using Library.Exceptions;
using Library.Results;
using Library.Threading.MultistepProgress;
using Library.Validations;
using Library.Wpf.Dialogs;
using Library.Wpf.Windows;
using Library.Wpf.Windows.CommonTools;

using Microsoft.WindowsAPICodePack.Dialogs;

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows;

using UI;

namespace HanyCo.Infra.UI.Pages;

/// <summary>
/// Interaction logic for FunctionalityEditorPage.xaml
/// </summary>
public sealed partial class FunctionalityEditorPage : IStatefulPage, IAsyncSavePage
{
    private readonly IFunctionalityCodeService _codeService;
    private readonly IDbTableService _dbTableService;
    private readonly IDtoService _dtoService;
    private readonly IModuleService _moduleService;
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
        (this._service, this._codeService) = (service, codeService);
        this._moduleService = moduleService;
        this._dbTableService = dbTableService;
        this._reporter = reporter;
        this._dtoService = dtoService;

        this.InitializeComponent();
    }

    public bool IsViewModelChanged { get; set; }

    public FunctionalityViewModel? ViewModel
    {
        get => this.GetViewModelByDataContext<FunctionalityViewModel>();
        set => this.SetViewModelByDataContext(value, () =>
        {
            this.DtoViewModelEditor.IsEnabled = this.ViewModel?.SourceDto != null;
            if (this.ViewModel?.SourceDto is null)
            {
                this.ComponentCodeResultUserControl.Codes = Codes.Empty;
            }
        });
    }

    public async Task<Result> SaveToDbAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            this.EnableActors(false);

            await this
                .PrepareViewModel()
                .ValidateFormAsync(cancellationToken)
                .ThrowOnFailAsync(this.Title, cancellationToken: cancellationToken)
                .ThrowIfCancellationRequested(cancellationToken)
                .End();

            var result = await this._service
                .SaveViewModelAsync(this.ViewModel, cancellationToken: cancellationToken)
                .ThrowOnFailAsync(this, "Error occurred while saving functionality to database.", cancellationToken: cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            return Result.Fail(ex);
        }
        finally
        {
            this.EnableActors(true);
        }
    }

    protected override Task<Result> OnValidateFormAsync(CancellationToken cancellationToken = default)
    {
        this.CheckIfInitiated();
        return base.OnValidateFormAsync(cancellationToken);
    }

    [MemberNotNull(nameof(ViewModel))]
    private void CheckIfInitiated(bool fullCheck = true)
    {
        var initiated = fullCheck
            ? this.ViewModel is not null and { Module: not null } and { Controller: not null } and { BlazorDetailsComponent: not null } and { GetAllQuery: not null }
            : this.ViewModel is not null;

        Check.MustBe(initiated, () => "Please create a new Functionality or edit an old one.");
    }

    private async void CreateFunctionalityButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            this.EnableActors(false);
            await CreateFunctionality();
        }
        finally
        {
            this.EnableActors(true);
        }
    }

    private async Task CreateFunctionality(CancellationToken cancellationToken = default)
    {
        await this.AskToSaveIfChangedAsync(cancellationToken: cancellationToken).BreakOnFail().End();
        this.ViewModel = null;
        this.ViewModel = await this.GetNewViewModel(cancellationToken);
    }

    private async void DeleteFunctionalityButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            this.EnableActors(false);
            await DeleteFunctionality();
        }
        finally
        {
            this.EnableActors(true);
        }
    }

    private async Task DeleteFunctionality(CancellationToken cancellationToken = default)
    {
        var functionality = this.FunctionalityTreeView.SelectedItem;
        Check.MustBeNotNull(functionality, () => new CommonException("No functionality selected.", "Please select functionality", details: "If there is not functionality, please create one"));
        var resp = MsgBox2.AskWithWarn("Are you sure you want to delete this Functionality?", "This operation cannot be undone.", detailsExpandedText: "Any DTO, View Model and CQRS segregation associated to this Functionality will be deleted.");
        if (resp != TaskDialogResult.Yes)
        {
            return;
        }

        _ = await this._service.DeleteAsync(functionality, cancellationToken: cancellationToken).ShowOrThrowAsync(this.Title);
        await this.BindDataAsync();
    }

    private async void EditFunctionalityButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            this.EnableActors(false);
            await LoadFunctionality();            
        }
        finally
        {
            this.EnableActors(true);
        }
    }

    private async Task LoadFunctionality(CancellationToken cancellation = default)
    {
        var id = this.FunctionalityTreeView.SelectedItem
                        .Check()
                        .NotNull(() => "No functionality is selected.")
                        .NotNull(x => x!.Id).ThrowOnFail()
                        .Value!.Id!.Value;
        await this.AskToSaveIfChangedAsync(cancellationToken: cancellation).BreakOnFail().End();
        var viewModel = await this._service.GetByIdAsync(id, cancellation);
        this.ViewModel = viewModel;
        await this.BindDataAsync();
        CheckIfInitiated(true);
    }

    private void EnableActors(bool enable)
    {
        this.CreateFunctionalityButton.IsEnabled = enable;
        this.EditFunctionalityButton.IsEnabled = enable;
        this.DeleteFunctionalityButton.IsEnabled = enable;
        this.GenerateViewModelButton.IsEnabled = enable;
        this.GenerateCodesButton.IsEnabled = enable;
        this.SaveToDbButton.IsEnabled = enable;
        this.SaveToDiskButton.IsEnabled = enable;
        this.MainTabControl.IsEnabled = enable;
        this.SelectRootDtoByDtoButton.IsEnabled = enable;
        this.SelectRootDtoByTableButton.IsEnabled = enable;
        this.DtoViewModelEditor.IsEnabled = enable;
        App.Current.DoEvents();
    }

    private async void GenerateCodesButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            this.EnableActors(false);
            await this.ValidateFormAsync().ThrowOnFailAsync(this.Title).End();
            this.ComponentCodeResultUserControl.Codes = this.ActionScopeRun(() => this._codeService.GenerateCodes(this.ViewModel!, new()), "Generating code...").ThrowOnFail(this.Title);
        }
        finally
        {
            this.EnableActors(true);
        }
    }

    private async void GenerateViewModelButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            this.EnableActors(false);
            this.CheckIfInitiated(false);
            this.PrepareViewModel();
            this.ViewModel = await this._service.GenerateViewModelAsync(this.ViewModel).ThrowOnFailAsync(this.Title);
        }
        finally
        {
            this.EnableActors(true);
        }
    }

    private async Task<FunctionalityViewModel> GetNewViewModel(CancellationToken cancellationToken = default) =>
        await this._service.CreateAsync(cancellationToken).WithAsync(x => x.SourceDto.NameSpace = SettingsService.Get().productName ?? string.Empty);

    private async void Me_Loaded(object sender, RoutedEventArgs e)
    {
        this.GetChildren<IAsyncBindable>().ForEach(async x => await x.BindAsync());
        this.ViewModel = await this.GetNewViewModel();
    }

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
    private FunctionalityEditorPage PrepareViewModel()
    {
        this.ViewModel!.Name ??= this.ViewModel.SourceDto.Name?.TrimEnd("Dto".ToCharArray()).AddToEnd("Functionality");
        return this;
    }

    private void PrepareViewModelByDto(in DtoViewModel? details)
    {
        this.CheckIfInitiated(false);

        this.ViewModel.SourceDto = null!;

        //Optional! To make sure that the selected dto exists and has details.
        if (details == null)
        {
            return;
        }

        this.ViewModel.SourceDto = details;
        //Maybe the user filled these data. We shouldn't overwrite user's preferences. If user
        // presses <Reset> button, user's preferences will be cleaned.
        //if (this.ViewModel.NameSpace.IsNullOrEmpty())
        //{
        //    this.ViewModel.NameSpace = this.ViewModel.SourceDto.NameSpace = details.NameSpace ?? SettingsService.Get().productName;
        //}
        if (this.ViewModel.SourceDto.NameSpace.IsNullOrEmpty())
        {
            this.ViewModel.SourceDto.NameSpace = details.NameSpace ?? SettingsService.Get().productName ?? string.Empty;
        }

        if (this.ViewModel.Name.IsNullOrEmpty())
        {
            this.ViewModel.Name = details.Name?.TrimEnd([.. "Dto"]).AddToEnd("Functionality");
        }
        //The form is now ready to call services.
    }

    private async Task<Result<string?>> SaveCodes(CancellationToken cancellationToken = default)
    {
        if (!ResultHelper.TryParse(await this.ValidateFormAsync(cancellationToken), out var validationResult))
        {
            return validationResult.WithValue<string?>(null);
        }
        var codes = this.ViewModel!.Codes.SelectAll().Compact();
        if (!codes.Any())
        {
            return Result.Fail("No source code found. Please press <Generate Sources> button.", string.Empty);
        }
        var settings = SettingsService.Get();
        var dir = settings.projectSourceRoot;
        if (Directory.Exists(dir) && Directory.GetFileSystemEntries(dir).Any())
        {
            try
            {
                var resp = MsgBox2.AskWithCancel("Source root folder is not empty.", $"{dir} has already some  content. Do you want to delete it's contents?", "Source folder not empty");
                if (resp == TaskDialogResult.Cancel)
                {
                    return Result.Fail<string>(new OperationCancelledException());
                }
                if (resp == TaskDialogResult.Yes)
                {
                    Directory.Delete(dir, true);
                }
            }
            finally
            {
                await Task.Delay(750, cancellationToken);
            }
        }
        var files = codes.Select(code => (Path.Combine(getPath(settings, code), code.FileName), code.Statement));
        var saveResult = FileUiTools.SaveToFile(files, $"Saving source codes to {settings.projectSourceRoot}");
        await Application.Current.DoEventsAsync(500);
        return saveResult.WithValue(settings.projectSourceRoot).OnSucceed(x => x.SetMessage("Codes are saved successfully."));

        static string getPath(SettingsModel settings, Code code)
        {
            Result<string?> relativePath = code.props().Category switch
            {
                CodeCategory.Dto => settings.dtosPath ?? "Domain/Dtos",

                CodeCategory.Query => settings.queriesPath ?? "Application/Queries",
                CodeCategory.Command => settings.commandsPath ?? "Application/Commands",

                CodeCategory.ViewModel => settings.blazorPagesPath ?? "UI/ViewModels",
                CodeCategory.Page => settings.blazorPagesPath ?? "UI/Pages",
                CodeCategory.Component => settings.blazorComponentsPath ?? "UI/Components",

                CodeCategory.Api => settings.controllersPath ?? "API/Controllers",

                CodeCategory.Converter => settings.convertersPath ?? "Infrastructure/Converters",

                _ => Result.Fail(new NotSupportedException("Code category is null or not supported."), string.Empty)
            };
            relativePath.ThrowOnFail().End();
            return Path.Combine(settings.projectSourceRoot.NotNull(), relativePath.Value.NotNull());
        }
    }

    private async void SaveToDbButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            this.EnableActors(false);
            _ = ControlHelper.MoveToNextUIElement();
            await this.SaveToDbAsync().ShowOrThrowAsync(this.Title).End();
            await this.FunctionalityTreeView.BindAsync();
        }
        finally
        {
            this.EnableActors(true);
        }
    }

    private async void SaveToDiskButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            this.EnableActors(false);
            await SaveToDisk();
        }
        finally
        {
            this.EnableActors(true);
        }
    }

    private async Task SaveToDisk(CancellationToken cancellationToken = default)
    {
        _ = ControlHelper.MoveToNextUIElement();
        var saveResult = await this.SaveCodes(cancellationToken).ThrowOnFailAsync(this.Title);
        SourceCodeHelper.ShowDiskOperationResult(saveResult);
    }

    private async void SelectRootDtoByDtoButton_Click(object sender, RoutedEventArgs e)
    {
        this.EnableActors(false);
        try
        {
            _ = await this.AskToSaveIfChangedAsync().BreakOnFail();

            //Let user to select a DTO
            this._dtoExplorerTreeView ??= new CqrsExplorerTreeView { LoadDtos = true };
            _ = HostDialog.ShowDialog(this._dtoExplorerTreeView, "Select Root DTO", "Select a DTO to create a Functionality.", _ => Check.If(this._dtoExplorerTreeView.SelectedItem is not DtoViewModel, () => "Please select a DTO.")).BreakOnFail();

            //Did user select a DTO?
            if (this._dtoExplorerTreeView.SelectedItem is DtoViewModel selectedDto)
            {
                var dto = await this._dtoService.GetByIdAsync(selectedDto.Id!.Value);
                this.PrepareViewModelByDto(dto);
            }
        }
        finally
        {
            this.EnableActors(true);
        }
    }

    private async void SelectRootDtoByTableButton_Click(object sender, RoutedEventArgs e)
    {
        this.EnableActors(false);
        try
        {
            await this.AskToSaveIfChangedAsync().BreakOnFail().End();

            // Let user to select a table
            this._databaseExplorerUserControl ??= await new DatabaseExplorerUserControl().InitializeAsync(this._dbTableService, this._reporter);
            _ = HostDialog.ShowDialog(this._databaseExplorerUserControl, "Select Root Table", "Select a table to create a Functionality.", _ => Check.If(this._databaseExplorerUserControl.SelectedTable is null, () => "Please select a table.")).BreakOnFail();

            // Did user select a table?
            if (this._databaseExplorerUserControl.SelectedTable is DbTableViewModel table)
            {
                var columns = await this._dbTableService.GetColumns(table.Name!);
                var dto = this._dtoService.CreateByDbTable(table, columns);
                this.PrepareViewModelByDto(dto);
            }
        }
        finally
        {
            this.EnableActors(true);
        }
    }
}