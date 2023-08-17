using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

using Contracts.Services;
using Contracts.ViewModels;

using HanyCo.Infra.UI.Dialogs;
using HanyCo.Infra.UI.Helpers;
using HanyCo.Infra.UI.Services;

using Library.Exceptions.Validations;
using Library.Results;
using Library.Validations;
using Library.Wpf.Dialogs;
using Library.Wpf.Windows;

using Microsoft.WindowsAPICodePack.Dialogs;

namespace HanyCo.Infra.UI.Pages;

/// <summary>
/// Interaction logic for BlazorCodeGenertorPage.xaml
/// </summary>
public partial class BlazorComponentGenertorPage : IStatefulPage, IAsyncSavePage
{
    #region Fields

    public static readonly DependencyProperty IsEditModeProperty = DependencyProperty.Register(
        "IsEditMode",
        typeof(bool),
        typeof(BlazorComponentGenertorPage),
        new PropertyMetadata(false));

    public static readonly DependencyProperty SelectedComponentProperty =
        DependencyProperty.Register("SelectedComponent", typeof(UiComponentViewModel), typeof(BlazorComponentGenertorPage), new PropertyMetadata(null));

    private readonly IBlazorCodingService _codeService;
    private readonly ICqrsCommandService _commandService;
    private readonly ICqrsQueryService _cqrsQueryService;
    private readonly IDtoService _dtoService;
    private readonly IModuleService _moduleService;
    private readonly IBlazorComponentService _service;

    #endregion Fields

    public BlazorComponentGenertorPage(
        IBlazorComponentService service,
        IBlazorCodingService codeService,
        IModuleService moduleService,
        ICqrsCommandService commandService,
        ICqrsQueryService cqrsQueryService,
        IDtoService dtoService,
        ILogger logger)
        : base(logger)
    {
        this._service = service;
        this._codeService = codeService;
        this._moduleService = moduleService;
        this._commandService = commandService;
        this._cqrsQueryService = cqrsQueryService;
        this._dtoService = dtoService;
        this.InitializeComponent();
    }

    public bool IsEditMode { get => (bool)this.GetValue(IsEditModeProperty); set => this.SetValue(IsEditModeProperty, value); }

    bool IStatefulPage.IsViewModelChanged { get; set; }

    public UiComponentViewModel? SelectedComponent { get => (UiComponentViewModel?)this.GetValue(SelectedComponentProperty); set => this.SetValue(SelectedComponentProperty, value); }

    public UiComponentViewModel? ViewModel
    {
        get => this.DataContext.Cast().As<UiComponentViewModel>();
        set
        {
            if (this.ViewModel is not null)
            {
                this.ViewModel.PropertyChanged -= this.ViewModel_PropertyChanged;
            }
            this.DataContext = value;
            if (this.ViewModel is not null)
            {
                _ = this.ViewModel.HandlePropertyChanges(this.ViewModel_PropertyChanged);
            }
        }
    }

    public async Task<Result<int>> SaveDbAsync()
    {
        try
        {
            this.SaveToDbButton.IsEnabled = false;
            this.ValidateForm(false);
            var scope = this.ActionScopeBegin("Saving...");
            var saveResult = await this._service.SaveViewModelAsync(this.ViewModel).ThrowOnFailAsync();
            ((IStatefulPage)this).IsViewModelChanged = false;
            await this.BindComponentTreeView();
            this.IsEditMode = true;
            scope.End(saveResult);
            return Result<int>.CreateSuccess(1);
        }
        finally
        {
            this.SaveToDbButton.IsEnabled = true;
        }
    }

    protected override Task<Result> OnValidateFormAsync()
    {
        this.ValidateForm(true);
        return base.OnValidateFormAsync();
    }

    private async Task BindComponentTreeView()
    {
        this.Debug("Binding ComponentTreeView");
        var components = await this._service.GetAllAsync();
        _ = this.ComponentTreeView.BindItems(components);
    }

    private async void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        await this.ResetFormAsync();
        await this.BindComponentTreeView();
        this.EndActionScope();
    }

    private async void ComponentTreeView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        => await this.LoadBlazorComponent();

    private void ComponentTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        => this.SelectedComponent = e.NewValue.Cast().As<TreeViewItem>()?.DataContext.Cast().As<UiComponentViewModel>();

    private async void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        Check.MustBeNotNull(this.SelectedComponent, () => new ValidationException("Please select a component to delete."));
        if (this.SelectedComponent.Id is not { } id)
        {
            return;
        }
        if (MsgBox2.AskWithWarn(text: $"Are you sure you want to permanently delete '{this.SelectedComponent.Name}'?") != TaskDialogResult.Yes)
        {
            return;
        }
        this.Debug($"Component {this.SelectedComponent.Name} deleting.");
        _ = await this._service.DeleteAsync(this.SelectedComponent).ThrowOnFailAsync();
        this.Debug($"Component {this.SelectedComponent.Name} deleted.");
        await this.ResetFormAsync();
        await this.BindComponentTreeView();
        this.EndActionScope();
    }

    private async void EditBlazorComponentButton_Click(object sender, RoutedEventArgs e)
        => await this.LoadBlazorComponent();

    private async void GenerateCodeButton_Click(object sender, RoutedEventArgs e)
    {
        _ = await this.ValidateFormAsync().ThrowOnFailAsync();
        this.Debug("Generating code…");

        var model = this.ViewModel!;
        await fillActionCqrsInfo(model);
        var codes = this._codeService.GenerateCodes(model, new(model.GenerateMainCode, model.GeneratePartialCode, model.GenerateUiCode));
        this.ComponentCodeResultUserControl.Codes = codes;
        this.ResultsTabItem.IsSelected = true;

        this.Debug("Code generated.");

        async Task fillActionCqrsInfo(UiComponentViewModel model)
        {
            foreach (var action in model.UiActions.Where(x => x.CqrsSegregate is not null))
            {
                action.CqrsSegregate = await action.CqrsSegregate.FillAsync(this._cqrsQueryService, this._commandService);
            }
        }
    }

    private async Task LoadBlazorComponent()
    {
        Check.MustBeNotNull(this.SelectedComponent, () => "Please select a component to edit.");
        _ = await this.AskToSaveIfChangedAsync().BreakOnFail();

        var scope = this.ActionScopeBegin();
        this.EditBlazorComponentButton.IsEnabled = false;
        await this.ResetFormAsync();
        this.ViewModel = await getViewModel();
        this.IsEditMode = true;
        scope.End();

        async Task<UiComponentViewModel?> getViewModel()
        {
            var result = await this._service.GetByIdAsync(this.SelectedComponent.Id!.Value);
            Check.MustBeNotNull(result, () => new NotFoundValidationException("Component not found"));
            if (result.PageDataContext?.Id is { } id)
            {
                var dataContext = await this._dtoService.GetByIdAsync(id);
                result.ValidateOnPropertySet = false;
                result.PageDataContext = dataContext!;
                result.ValidateOnPropertySet = true;
            }
            return result;
        }
    }

    private async void Me_BindingData(object sender, EventArgs e)
    {
        var scope = this.ActionScopeBegin("Initializing…");

        this.ComponentDetailsUserControl.Initialize(this._codeService, this._moduleService);
        this.ComponentPropertiesUserControl.Initialize(this._codeService);
        this.ComponentActionsUserControl.Initialize(this._codeService);
        this.ComponentCodeResultUserControl.Initialize(this._codeService);
        await this.BindComponentTreeView();

        this.IsEditMode = false;

        scope.End();
    }

    private async void NewBlazorComponentButton_Click(object sender, RoutedEventArgs e)
    {
        _ = await this.AskToSaveIfChangedAsync().BreakOnFail();
        var resp = SelectCqrsDialog.Show<DtoViewModel>(new("Select Page ViewModel", SelectCqrsDialog.LoadEntity.Dto, SelectCqrsDialog.FilterDto.ViewModel));
        if (resp is null or { IsSucceed: false } or { Value: null } || resp.Value is not DtoViewModel dto)
        {
            return;
        }

        var scope = this.ActionScopeBegin("Creating new component...");
        this.ViewModel = await this._codeService.CreateNewComponentByDtoAsync(dto);
        this.IsEditMode = true;
        scope.End();
    }

    private async void NewBlazorComponentUnboundButton_Click(object sender, RoutedEventArgs e)
    {
        this.ViewModel = await this._codeService.CreateNewComponentAsync();
        this.IsEditMode = true;
        this.EndActionScope();
    }

    private async Task ResetFormAsync()
    {
        _ = await this.AskToSaveIfChangedAsync().BreakOnFail();
        this.ViewModel = null;
        this.IsEditMode = false;
        ((IStatefulPage)this).IsViewModelChanged = false;
        this.ComponentCodeResultUserControl.Codes = null;
        this.EndActionScope();
    }

    private async void SaveToDbButton_Click(object sender, RoutedEventArgs e)
        => await this.SaveDbAsync();

    private async void SaveToFileButton_Click(object sender, RoutedEventArgs e)
    {
        _ = await this.ValidateFormAsync();
        var viewModel = this.ViewModel!;
        var codes = this._codeService.GenerateCodes(viewModel, new(viewModel.GenerateMainCode, viewModel.GeneratePartialCode, viewModel.GenerateUiCode));
        _ = await codes.Value.SaveToFileAsync().ThrowOnFailAsync();
    }

    private async void ValidateButton_Click(object sender, RoutedEventArgs e)
    {
        _ = await this.ValidateFormAsync();
        MsgBox2.Inform("✔ Everything is ok.");
    }

    [MemberNotNull(nameof(ViewModel))]
    private void ValidateForm(bool toGenerateCode)
    {
        _ = this.ViewModel.NotNull(() => new ValidationException("Please create/edit a component.")).Check(CheckBehavior.ThrowOnFail)
                          .NotNull(x => this.ViewModel)
                          .NotNull(x => this.ViewModel.Name)
                          .NotNull(x => this.ViewModel.NameSpace);
        if (toGenerateCode)
        {
            if (!this.ViewModel.GenerateMainCode && !this.ViewModel.GeneratePartialCode && !this.ViewModel.GenerateUiCode)
            {
                throw new ValidationException("Please select a code generation option at least.", "No code generation option is selected.", owner: this);
            }
        }
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        => ((IStatefulPage)this).IsViewModelChanged = true;
}