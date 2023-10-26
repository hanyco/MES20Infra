using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Controls;

using Contracts.Services;
using Contracts.ViewModels;

using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.UI.Helpers;
using HanyCo.Infra.UI.ViewModels;

using Library.BusinessServices;
using Library.EventsArgs;
using Library.Results;
using Library.Validations;
using Library.Wpf.Dialogs;
using Library.Wpf.Windows;

using Microsoft.WindowsAPICodePack.Dialogs;

namespace HanyCo.Infra.UI.Pages;

/// <summary>
/// Interaction logic for CqrsCommandDetailsPage.xaml
/// </summary>
public partial class CqrsCommandDetailsPage : IStatefulPage, IAsyncSavePage
{
    private readonly ICqrsCodeGeneratorService _codeGeneratorService;
    private readonly IDtoService _dtoService;
    private readonly IModuleService _moduleService;
    private readonly ICqrsCommandService _service;

    public CqrsCommandDetailsPage(
        ICqrsCommandService CommandService,
        IDtoService dtoService,
        IModuleService moduleService,
        ILogger logger,
        ICqrsCodeGeneratorService generatorService)
        : base(logger)
    {
        this._service = CommandService;
        this._dtoService = dtoService;
        this._moduleService = moduleService;
        this._codeGeneratorService = generatorService;
        this.DataContextChanged += this.CqrsCommandDetailsPage_DataContextChanged;
        this.InitializeComponent();
    }

    public bool IsViewModelChanged { set; get; }

    public CqrsCommandViewModel? ViewModel
    {
        get => this.DataContext.Cast().As<CqrsCommandViewModel>();
        set => this.DataContext = value;
    }

    public async Task<Result> SaveDbAsync()
    {
        Check.MutBeNotNull(this.ViewModel);
        try
        {
            this.ViewModel.SecurityClaims = this.SecurityClaimCollectorUserControl.ClaimViewModels;
            var result = await this._service.SaveViewModelAsync(this.ViewModel);
            if (result)
            {
                this.IsViewModelChanged = false;
                this.Logger.Debug("CQRS Command saved.");
            }
            return result;
        }
        finally
        {
            await this.InitCommandsTreeViewAsync();
        }
    }

    private async void CommandsTreeView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        => await this.LoadCommandViewModelAsync();

    private void CommandsTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        var cCommand = e.NewValue.Cast().As<TreeViewItem>()?.DataContext.Cast().As<CqrsCommandViewModel>();
        this.EditCommandButton.IsEnabled = this.DeleteCommandButton.IsEnabled = cCommand is not null;
    }

    private void CqrsCommandDetailsPage_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        => this.RefreshFormState();

    private async void DeleteCommandButton_Click(object sender, RoutedEventArgs e)
    {
        var model = this.CommandsTreeView.GetSelectedModel<CqrsCommandViewModel>();
        if (model is null)
        {
            throw new ValidationException("Please select a Command.");
        }

        if (MsgBox2.AskWithWarn("Are you sure to delete this item?") != TaskDialogResult.Yes)
        {
            return;
        }

        _ = await this._service.DeleteAsync(model);
        await this.InitCommandsTreeViewAsync();
        this.IsViewModelChanged = false;
        this.NewCommandButton.PerformClick();
    }

    private async void EditCommandButton_Click(object sender, RoutedEventArgs e)
        => await this.LoadCommandViewModelAsync();

    private async void GenerateCodeButton_Click(object sender, RoutedEventArgs e)
    {
        _ = this.ViewModel.NotNull().Check()
                .NotNull(x => this.ViewModel.ParamsDto)
                .NotNull(x => this.ViewModel.ParamsDto.Id)
                .NotNull(x => this.ViewModel.ResultDto)
                .NotNull(x => this.ViewModel.ResultDto.Id)
                .ThrowOnFail();
        var props = await this._dtoService.GetPropertiesByDtoIdAsync(this.ViewModel.ParamsDto.Id!.Value);
        _ = this.ViewModel.ParamsDto.Properties.ClearAndAddRange(props);
        props = await this._dtoService.GetPropertiesByDtoIdAsync(this.ViewModel.ResultDto.Id!.Value);
        _ = this.ViewModel.ResultDto.Properties.ClearAndAddRange(props);
        var codes = await this._codeGeneratorService.GenerateCodesAsync(this.ViewModel);
        this.ComponentCodeResultUserControl.Codes = codes;
        this.ResultsTabItem.IsSelected = true;
    }

    private async Task InitCommandsTreeViewAsync()
    {
        var cCommands = await this._service.GetAllAsync();
        _ = this.CommandsTreeView.BindItems(cCommands);
    }

    private void InitFormInfo() =>
        this.CategoryComboBox.BindItemsSourceToEnum<CqrsSegregateCategory>(CqrsSegregateCategory.Create);

    private async Task LoadCommandViewModelAsync()
    {
        if (await this.AskToSaveIfChangedAsync() != true)
        {
            return;
        }
        var selectedViewModel = this.CommandsTreeView.GetSelectedModel<CqrsCommandViewModel>();
        Check.MustBe(selectedViewModel?.Id is not null, () => new ValidationException("Please select a Command."));
        
        var viewModel = await this._service.FillByDbEntity(selectedViewModel, selectedViewModel.Id.Value);
        Check.MustBeNotNull(viewModel, () => "ID not found");
        
        this.ViewModel = viewModel.HandlePropertyChanges(this.ViewModel_PropertyChanged);
        this.SecurityClaimCollectorUserControl.ClaimViewModels = this.ViewModel.SecurityClaims;
    }

    private void Me_Loaded(object sender, RoutedEventArgs e) => 
        this.SecurityClaimCollectorUserControl.HandleAutoGenerateClaimEvent(this.SecurityClaimCollectorUserControl_OnAutoGenerateClaim);

    private async void NewCommandButton_Click(object sender, RoutedEventArgs e)
            => await this.NewCommandViewModelAsync();

    private async Task NewCommandViewModelAsync()
    {
        if (await this.AskToSaveIfChangedAsync() != true)
        {
            return;
        }

        this.IsViewModelChanged = false;
        this.ViewModel = new CqrsCommandViewModel().HandlePropertyChanges(this.ViewModel_PropertyChanged);
        await this.BindDataAsync();
    }

    private async void PageBase_Binding(object sender, EventArgs e)
    {
        await this.InitCommandsTreeViewAsync();
        this.InitFormInfo();
        this.RefreshFormState();
    }

    private void RefreshFormState()
    {
        this.NewCommandButton.IsEnabled = this.EditCommandButton.IsEnabled = this.DeleteCommandButton.IsEnabled = this.ViewModel is null;
        this.SaveAllToDiskButton.IsEnabled = this.SaveToDbButton.IsEnabled = this.GenerateCodeButton.IsEnabled = this.ResetFormButton.IsEnabled = this.ViewModel is not null;
        this.CommandDetailsGrid.IsEnabled = this.ViewModel is not null;
    }

    private async void ResetFormButton_Click(object sender, RoutedEventArgs e)
    {
        if (await this.AskToSaveIfChangedAsync() != true)
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
        var codes = await this._codeGeneratorService.GenerateCodesAsync(this.ViewModel);
        _ = await SourceCodeHelper.SaveToFileAskAsync(codes);
    }

    private async void SaveToDbButton_Click(object sender, RoutedEventArgs e)
    {
        _ = this.SelectModuleBox.Focus();
        _ = await this.SaveDbAsync();
    }

    private IEnumerable<ClaimViewModel> SecurityClaimCollectorUserControl_OnAutoGenerateClaim() =>
        this.ViewModel?.Name.IsNullOrEmpty() ?? true
            ? Enumerable.Empty<ClaimViewModel>()
            : EnumerableHelper.Iterate(new ClaimViewModel { Key = this.ViewModel.Name });

    private async void SelectModuleBox_SelectedModuleChanged(object sender, ItemActedEventArgs<ModuleViewModel> e)
    {
        this.ResultDtoComboBox.ItemsSource = null;
        var moduleId = e.Item?.Id;
        if (moduleId is null)
        {
            return;
        }
        var dtos = await this._dtoService.GetByModuleId(moduleId.Value);

        var paramDtos = dtos.Where(x => x.IsParamsDto).OrderBy(x => x.Name).ToList();
        _ = this.ParamDtoComboBox.BindItemsSource(paramDtos, nameof(ModuleViewModel.Name), this.ViewModel?.ParamsDto);

        var resultDtos = dtos.Where(x => x.IsResultDto).OrderBy(x => x.Name).ToList();
        _ = this.ResultDtoComboBox.BindItemsSource(resultDtos, nameof(ModuleViewModel.Name), this.ViewModel?.ResultDto);
    }

    //private void ModuleComboBox_Initializing(object sender, InitialItemEventArgs<IModuleService> e)
    //    => e.Item = this._moduleService;

    private void SelectModuleUserControl_Initializing(object sender, InitialItemEventArgs<IModuleService> e)
        => e.Item = this._moduleService;

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
            => this.IsViewModelChanged = true;
}