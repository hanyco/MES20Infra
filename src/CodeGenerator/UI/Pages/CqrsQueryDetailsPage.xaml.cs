using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Controls;

using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.UI.Helpers;
using HanyCo.Infra.UI.Services;
using HanyCo.Infra.UI.ViewModels;

using Library.EventsArgs;
using Library.Results;
using Library.Validations;
using Library.Wpf.Dialogs;
using Library.Wpf.Windows;

using Microsoft.WindowsAPICodePack.Dialogs;

namespace HanyCo.Infra.UI.Pages;

/// <summary>
/// Interaction logic for CqrsQueryDetailsPage.xaml
/// </summary>
public partial class CqrsQueryDetailsPage : IStatefulPage, IAsyncSavePage
{
    private readonly ICqrsCodeGeneratorService _codeGeneratorService;
    private readonly IDtoService _dtoService;
    private readonly IModuleService _moduleService;
    private readonly ICqrsQueryService _service;

    public CqrsQueryDetailsPage(
        ICqrsQueryService queryService,
        IDtoService dtoService,
        IModuleService moduleService,
        ILogger logger,
        ICqrsCodeGeneratorService generatorService)
        : base(logger)
    {
        this._service = queryService;
        this._dtoService = dtoService;
        this._moduleService = moduleService;
        this._codeGeneratorService = generatorService;
        this.DataContextChanged += this.CqrsQueryDetailsPage_DataContextChanged;
        this.InitializeComponent();
    }

    public bool IsViewModelChanged { get; set; }

    public CqrsQueryViewModel? ViewModel
    {
        get => this.DataContext.CastAs<CqrsQueryViewModel>();
        set => this.DataContext = value;
    }

    public async Task<Result<int>> SaveAsync()
    {
        Check.NotNull(this.ViewModel);
        try
        {
            _ = await this._service.SaveViewModelAsync(this.ViewModel);

            this.IsViewModelChanged = false;
            this.Logger.Debug("CQRS Query saved.");
            return Result<int>.CreateSuccess(1);
        }
        finally
        {
            await this.InitQueriesTreeViewAsync();
        }
    }

    private void CqrsQueryDetailsPage_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        => this.RefreshFormState();

    private async void DeleteQueryButton_Click(object sender, RoutedEventArgs e)
    {
        var id = this.QueriesTreeView.GetSelectedValue<CqrsQueryViewModel>()?.Id;
        if (id is null)
        {
            throw new ValidationException("Please select a Query.");
        }

        if (MsgBox2.AskWithWarn("Are you sure to delete this item?") != TaskDialogResult.Yes)
        {
            return;
        }

        _ = await this._service.DeleteByIdAsync(id.Value);
        await this.InitQueriesTreeViewAsync();
        this.IsViewModelChanged = false;
        this.NewQueryButton.PerformClick();
    }

    private async void EditQueryButton_Click(object sender, RoutedEventArgs e)
        => await this.LoadQueryViewModelAsync();

    private async void GenerateCodeButton_Click(object sender, RoutedEventArgs e)
    {
        _ = this.ViewModel.Check(CheckBehavior.ThrowOnFail).NotNull()
            .NotNull(x => x.ParamDto)
            .NotNull(x => x.ParamDto.Id)
            .NotNull(x => x.ResultDto)
            .NotNull(x => x.ResultDto.Id);
        IEnumerable<PropertyViewModel> props = await this._dtoService.GetPropertiesByDtoIdAsync(this.ViewModel.ParamDto.Id.Value);
        _ = this.ViewModel.ParamDto.Properties.ClearAndAddRange(props);
        props = await this._dtoService.GetPropertiesByDtoIdAsync(this.ViewModel.ResultDto.Id.Value);
        _ = this.ViewModel.ResultDto.Properties.ClearAndAddRange(props);
        var codes = await this._codeGeneratorService.GenerateCodeAsync(this.ViewModel);
        this.ComponentCodeResultUserControl.Codes = codes;
        this.ResultsTabItem.IsSelected = true;
    }

    private void InitFormInfo()
        => this.CategoryComboBox.BindItemsSourceToEnum<CqrsSegregateCategory>(CqrsSegregateCategory.Create);

    private async Task InitQueriesTreeViewAsync()
        => this.QueriesTreeView.BindItems((IReadOnlyList<CqrsQueryViewModel>?)await this._service.GetAllAsync());

    private async Task LoadQueryViewModelAsync()
    {
        if (await this.AskToSaveAsync() != true)
        {
            return;
        }
        var selectedViewModel = this.QueriesTreeView.GetSelectedValue<CqrsQueryViewModel>();
        Check.If(selectedViewModel?.Id is not null, () => new ValidationException("Please select a Query."));
        this.Logger.Debug("Loading...");
        var viewModel = await this._service.FillByDbEntity(selectedViewModel, selectedViewModel.Id.Value);
        Check.NotNull(viewModel, () => "ID not found");
        this.ViewModel = viewModel.HandlePropertyChanges(this.ViewModel_PropertyChanged);
        this.Logger.Debug("Ready.");
    }

    private async void NewQueryButton_Click(object sender, RoutedEventArgs e)
        => await this.NewQueryViewModelAsync();

    private async Task NewQueryViewModelAsync()
    {
        if (await this.AskToSaveAsync() != true)
        {
            return;
        }

        this.IsViewModelChanged = false;
        this.ViewModel = new CqrsQueryViewModel().HandlePropertyChanges(this.ViewModel_PropertyChanged);
        await this.BindDataAsync();
    }

    private async void PageBase_Binding(object sender, EventArgs e)
    {
        await this.InitQueriesTreeViewAsync();
        this.InitFormInfo();
        this.RefreshFormState();
    }

    private async void QueriesTreeView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        => await this.LoadQueryViewModelAsync();

    private void QueriesTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        var cQuery = e.NewValue.CastAs<TreeViewItem>()?.DataContext.CastAs<CqrsQueryViewModel>();
        this.EditQueryButton.IsEnabled = this.DeleteQueryButton.IsEnabled = cQuery is not null;
    }

    private void RefreshFormState()
    {
        this.NewQueryButton.IsEnabled = this.EditQueryButton.IsEnabled = this.DeleteQueryButton.IsEnabled = this.ViewModel is null;
        this.SaveAllToDiskButton.IsEnabled
            = this.SaveToDbButton.IsEnabled
            = this.GenerateCodeButton.IsEnabled
            = this.ResetFormButton.IsEnabled
            = this.ViewModel is not null;
        this.QueriesTreeView.IsEnabled = this.ViewModel is null;
        this.QueryDetailsGrid.IsEnabled = this.ViewModel is not null;
    }

    private async void ResetFormButton_Click(object sender, RoutedEventArgs e)
    {
        if (await this.AskToSaveAsync() != true)
        {
            return;
        }

        if (this.ViewModel is not null)
        {
            this.ViewModel.PropertyChanged -= this.ViewModel_PropertyChanged;
            this.ViewModel = null;
        }
        this.ComponentCodeResultUserControl.Codes = null;
    }

    private async void SaveAllToDiskButton_Click(object sender, RoutedEventArgs e)
    {
        _ = this.ViewModel.NotNull();
        var codes = await this._codeGeneratorService.GenerateCodeAsync(this.ViewModel);
        _ = await SourceCodeHelper.SaveToFileAsync(codes);
    }

    private async void SaveToDbButton_Click(object sender, RoutedEventArgs e)
    {
        _ = this.SelectModuleBox.Focus();
        _ = await this.SaveAsync();
    }

    private async void SelectModuleBox_SelectedModuleChanged(object sender, ItemActedEventArgs<ModuleViewModel> e)
    {
        this.ResultDtoComboBox.ItemsSource = null;
        this.ParamDtoComboBox.ItemsSource = null;
        var moduleId = e.Item?.Id;
        if (moduleId is null)
        {
            return;
        }
        var dtos = await this._dtoService.GetByModuleId(moduleId.Value);

        var paramDtos = dtos.Where(x => x.IsParamsDto).OrderBy(x => x.Name);
        _ = this.ParamDtoComboBox.BindItemsSource(paramDtos, nameof(ModuleViewModel.Name));

        var resultDtos = dtos.Where(x => x.IsResultDto).OrderBy(x => x.Name);
        _ = this.ResultDtoComboBox.BindItemsSource(resultDtos, nameof(ModuleViewModel.Name));
    }

    private void SelectModuleUserControl_Initializing(object sender, InitialItemEventArgs<IModuleService> e)
        => e.Item = this._moduleService;

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        => this.IsViewModelChanged = true;
}