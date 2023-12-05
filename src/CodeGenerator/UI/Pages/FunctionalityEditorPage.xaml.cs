﻿using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows;

using Contracts.Services;
using Contracts.ViewModels;

using HanyCo.Infra.CodeGeneration.Definitions;
using HanyCo.Infra.UI.Helpers;
using HanyCo.Infra.UI.UserControls;

using Library.BusinessServices;
using Library.CodeGeneration.Models;
using Library.EventsArgs;
using Library.Results;
using Library.Threading.MultistepProgress;
using Library.Validations;
using Library.Wpf.Dialogs;
using Library.Wpf.Windows;
using Library.Wpf.Windows.CommonTools;

using Microsoft.WindowsAPICodePack.Dialogs;

using UI;

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

    protected override async Task OnBindDataAsync() =>
        await base.OnBindDataAsync();

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
        await this.AskToSaveIfChangedAsync().BreakOnFail().EndAsync();
        this.ViewModel = null;
        this.ViewModel = await this.GetNewViewModelAsync();
    }

    private void DeleteFunctionalityButton_Click(object sender, RoutedEventArgs e)
    {
        //Check.MustBeNotNull(this.FunctionalityTreeView.SelectedItem, () => new CommonException("No functionality selected.", "Please select functionality", details: "If there is not functionality, please create one"));
        var resp = MsgBox2.AskWithWarn("Are you sure you want to delete this Functionality?", "This operation cannot be undone.", detailsExpandedText: "Any DTO, View Model and CQRS segregation associated to this Functionality will be deleted.");
        Check.If(resp != TaskDialogResult.Ok).BreakOnFail().End();
        //_ = await this._service.DeleteAsync(this.FunctionalityTreeView.SelectedItem).ShowOrThrowAsync(this.Title);
    }

    private async void GenerateCodesButton_Click(object sender, RoutedEventArgs e)
    {
        await this.ValidateFormAsync().ThrowOnFailAsync(this.Title).EndAsync();
        var codes = this.ActionScopeRun(() => this._codeService.GenerateCodes(this.ViewModel!, new(true)), "Generating code...").ThrowOnFail(this.Title);

        this.ComponentCodeResultUserControl.Codes = codes;
    }

    private async void GenerateViewModelButton_Click(object sender, RoutedEventArgs e)
    {
        await this.ValidateFormAsync().ThrowOnFailAsync(this.Title).EndAsync();
        this.PrepareViewModel();
        var viewModel = await this.ActionScopeRunAsync(() => this._service.GenerateViewModelAsync(this.ViewModel), "Generating view models...").ThrowOnFailAsync(this.Title);
        this.ViewModel = viewModel;
    }

    private async Task<FunctionalityViewModel> GetNewViewModelAsync() =>
        await this._service.CreateAsync().WithAsync(x => x.SourceDto.NameSpace = SettingsService.Get().productName ?? string.Empty);

    private async void Me_Loaded(object sender, RoutedEventArgs e) =>
        this.ViewModel = await this.GetNewViewModelAsync();

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
    private void PrepareViewModel() =>
        this.ViewModel!.Name = this.ViewModel.SourceDto.Name;

    private void PrepareViewModelByDto(DtoViewModel? details)
    {
        this.CheckIfInitiated();

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
            this.ViewModel.Name = details.Name;
        }
        //The form is now ready to call services.
    }

    private async Task<Result<string?>> SaveCodes()
    {
        if (!ResultHelper.TryParse(await this.ValidateFormAsync(), out var validationResult))
        {
            return validationResult.WithValue<string?>(null);
        }
        var codes = this.ViewModel!.Codes.SelectAll().Compact();
        if (!codes.Any())
        {
            return Result<string>.CreateFailure("No source code found. Please press <Generate Sources> button.", string.Empty);
        }
        var settings = SettingsService.Get();
        {
            var dir = settings.projectSourceRoot;
            if (Directory.Exists(dir))
            {
                if (Directory.GetFileSystemEntries(dir).Any())
                {
                    try
                    {
                        var resp = MsgBox2.AskWithCancel("Source root folder is not empty.", $"{dir} has already some  content. Do you want to delete it's contents?", "Source folder not empty");
                        if (resp == TaskDialogResult.Cancel)
                        {
                            return Result<string>.CreateFailure(new Library.Exceptions.OperationCancelException());
                        }
                        if (resp == TaskDialogResult.Yes)
                        {
                            Directory.Delete(dir, true);
                        }
                    }
                    finally
                    {
                        await Task.Delay(750);
                    }
                }
            }
        }
        var files = codes.Select(code => (Path.Combine(getPath(settings, code), code.FileName), code.Statement));
        var saveResult = FileUiTools.SaveToFile(files, $"Saving source codes to {settings.projectSourceRoot}");
        await App.Current.DoEventsAsync(500);
        return saveResult.WithValue(settings.projectSourceRoot).IfSucceed(x => x.SetMessage("Codes are saved successfully."));

        static string getPath(SettingsModel settings, Code code)
        {
            Result<string> relativePath = code.props().Category switch
            {
                CodeCategory.Dto => settings.dtosPath ?? "Dtos",
                CodeCategory.Query => settings.queriesPath ?? "Queries",
                CodeCategory.Command => settings.commandsPath ?? "Commands",
                CodeCategory.Page => settings.blazorPagesPath ?? "UI/Pages",
                CodeCategory.Component => settings.blazorComponentsPath ?? "UI/Components",
                CodeCategory.Converter => settings.convertersPath ?? "Converters",
                _ => Result<string>.CreateFailure(new NotSupportedException("Code category is null or not supported."), string.Empty)
            };
            relativePath.ThrowOnFail().End();
            var path = Path.Combine(settings.projectSourceRoot.NotNull(), relativePath.Value.NotNull());
            return path;
        }
    }

    private async void SaveToDbButton_Click(object sender, RoutedEventArgs e)
    {
        _ = ControlHelper.MoveToNextUIElement();
        _ = await this.SaveDbAsync().ShowOrThrowAsync(this.Title);
    }

    private async void SaveToDiskButton_Click(object sender, RoutedEventArgs e)
    {
        _ = ControlHelper.MoveToNextUIElement();
        var saveResult = await this.SaveCodes().ThrowOnFailAsync(this.Title);
        SourceCodeHelper.ShowDiskOperationResult(saveResult);
    }

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