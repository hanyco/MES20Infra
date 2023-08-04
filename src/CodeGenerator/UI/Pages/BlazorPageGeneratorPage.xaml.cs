using System.Diagnostics.CodeAnalysis;
using System.Windows;

using Contracts.Services;
using Contracts.ViewModels;

using HanyCo.Infra.UI.Dialogs;
using HanyCo.Infra.UI.Helpers;
using HanyCo.Infra.UI.ViewModels;

using Library.EventsArgs;
using Library.Exceptions.Validations;
using Library.Validations;
using Library.Wpf.Dialogs;
using Library.Wpf.Windows.Input.Commands;

using Microsoft.WindowsAPICodePack.Dialogs;

namespace HanyCo.Infra.UI.Pages;

/// <summary>
/// Interaction logic for BlazorPageGeneratorPage.xaml
/// </summary>
public partial class BlazorPageGeneratorPage
{
    #region Fields

    public static readonly DependencyProperty AllComponentsProperty
        = ControlHelper.GetDependencyProperty<IEnumerable<UiComponentViewModel>, BlazorPageGeneratorPage>(nameof(AllComponents));

    public static readonly DependencyProperty IsEditModeProperty = DependencyProperty.Register("IsEditMode", typeof(bool), typeof(BlazorPageGeneratorPage), new PropertyMetadata(false));

    public static readonly DependencyProperty SelectedComponentInAllProperty =
        ControlHelper.GetDependencyProperty<UiComponentViewModel, BlazorPageGeneratorPage>(nameof(SelectedComponentInAll));

    private readonly bool _canReset = true;
    private readonly IBlazorComponentService _componentService;
    private readonly IModuleService _moduleService;
    private readonly IBlazorPageService _service;
    private bool _canDelete;
    private bool _canEdit;

    #endregion Fields

    public static readonly LibRoutedUICommand SaveToDiskCommand = new();

    public BlazorPageGeneratorPage(IBlazorPageService service, IModuleService moduleService, IBlazorComponentService componentService, ILogger logger)
        : base(logger)
    {
        this._service = service;
        this._moduleService = moduleService;
        this._componentService = componentService;

        this.InitializeComponent();
    }

    public IEnumerable<UiComponentViewModel> AllComponents { get => (IEnumerable<UiComponentViewModel>)this.GetValue(AllComponentsProperty); set => this.SetValue(AllComponentsProperty, value); }

    public bool IsEditMode { get => (bool)this.GetValue(IsEditModeProperty); set => this.SetValue(IsEditModeProperty, value); }

    public UiComponentViewModel SelectedComponentInAll { get => (UiComponentViewModel)this.GetValue(SelectedComponentInAllProperty); set => this.SetValue(SelectedComponentInAllProperty, value); }

    public UiPageViewModel? ViewModel
    {
        get => this.DataContext.Cast().As<UiPageViewModel>();
        set
        {
            if (this.ViewModel is not null)
            {
                this.ViewModel.Components.CollectionChanged -= this.ViewModelComponents_CollectionChanged;
            }
            this.DataContext = value;
            if (this.ViewModel is not null)
            {
                this.ViewModel.Components.CollectionChanged += this.ViewModelComponents_CollectionChanged;
            }
        }
    }

    private async void AddComponentToPageButton_Click(object sender, RoutedEventArgs e)
    {
        var scope = this.BeginActionScope();
        if (this.ViewModel is null)
        {
            return;
        }
        if (this.SelectedComponentInAll is null)
        {
            return;
        }

        this.ViewModel.Components.Add(this.SelectedComponentInAll);
        await this.BindAllComponentsView();
        scope.End();
    }

    private async Task BindAllComponentsView()
    {
        this.AllComponents = Enumerable.Empty<UiComponentViewModel>();
        if (this.ViewModel?.Dto?.Id is not { } dtoId)
        {
            return;
        }

        //! شاید نیاز باشد یک کامپوننت، چند بار به پیج اضافه شود.
        this.AllComponents = await this._componentService.GetByPageDataContextIdAsync(dtoId);//x .Except(this.SelectedPage.Components);
    }

    private async Task BindPageTreeViewAsync()
    {
        var pages = await this._service.GetAllAsync();
        _ = this.PageTreeView.BindItems(pages);
        this.EndActionScope();
    }

    private async Task CreatePage()
    {
        if (!SelectCqrsDialog.Show<DtoViewModel>(out var result, new("Select ViewModel", SelectCqrsDialog.LoadEntity.Dto, SelectCqrsDialog.FilterDto.ViewModel)) || result is not { } dto)
        {
            return;
        }

        var viewModel = this._service.CreateViewModel(dto
            , dto.Name?.RemoveEnd("Dto")
                       .RemoveEnd("Params")
                       .RemoveEnd("Result")
                       .AddEnd("Page"));
        viewModel.Module = dto.Module;
        this.ViewModel = viewModel;
        await this.BindAllComponentsView();
        this.IsEditMode = true;
        this.EndActionScope();
    }

    private void DeleteBlazorPageCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        => e.CanExecute = !this.IsEditMode && this._canDelete;

    private async void DeleteBlazorPageCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        var page = ControlHelper.GetSelectedValue<UiPageViewModel>(this.PageTreeView);
        Check.MustBeNotNull(page, () => new ValidationException("Please select a page to edit."));
        if (MsgBox2.AskWithWarn("Are you sure you want to delete selected component?") != TaskDialogResult.Yes)
        {
            return;
        }

        var deleteResult = await this._service.DeleteAsync(page).ThrowOnFailAsync(this.Title);
        this.IsEditMode = false;
        await this.BindPageTreeViewAsync();
        this.EndActionScope(deleteResult);
    }

    private void EditBlazorPageCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        => e.CanExecute = !this.IsEditMode && this._canEdit;

    private async void EditBlazorPageCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        => await this.EditComponent();

    private async Task EditComponent()
    {
        if (this.IsEditMode)
        {
            return;
        }

        using var scope = this.BeginActionScope("Loading...");
        var id = ControlHelper.GetSelectedValue<UiPageViewModel>(this.PageTreeView)?.Id;
        Check.MustBeNotNull(id, () => "Please select a page to edit.");
        var viewModel = await this._service.GetByIdAsync(id.Value);
        this.ViewModel = viewModel;
        await this.BindAllComponentsView();
        this.IsEditMode = true;
        scope.End();
    }

    private void ElementPositionUserControl_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
    }

    private Task GenerateCodeAsync()
    {
        var scope = this.BeginActionScope("Generating code…");
        var model = this.ViewModel!;
        _ = this._service.CheckValidator(model);
        this.ComponentCodeResultUserControl.Codes = this._service.GenerateCodes(model, new(model.GenerateMainCode, model.GeneratePartialCode, model.GenerateUiCode));
        this.ResultsTabItem.IsSelected = true;
        this.Debug("Code generated.");
        scope.End();
        return Task.CompletedTask;
    }

    private void GenerateCodeCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        => e.CanExecute = this.IsEditMode;

    private async void GenerateCodeCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        => await this.GenerateCodeAsync();

    private void NewCommandBinding_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        => e.CanExecute = !this.IsEditMode;

    private async void NewCommandBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        => await this.Logger.LogBlockAsync(this.CreatePage);

    private async void Page_BindingData(object sender, EventArgs e)
    {
        await this.BindPageTreeViewAsync();
        this.EndActionScope();
    }

    private void PageComponentLisView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
    }

    private async void PageTreeView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        => await this.EditComponent();

    private void PageTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
            => this._canEdit = this._canDelete = !this.IsEditMode && e.NewValue is not null;


    private void RefreshPageComponents()
    {
        if (this.ViewModel?.Components?.Any() is not true)
        {
            return;
        }

        _ = this.PageComponentLisView.RebindItemsSource(this.ViewModel.Components);
    }

    private void RefreshPageComponentsButton_Click(object sender, RoutedEventArgs e)
        => this.RefreshPageComponents();

    private async void RemoveComponentFromPageButton_Click(object sender, RoutedEventArgs e)
    {
        if (this.ViewModel is null)
        {
            return;
        }
        var selectedComponent = this.PageComponentLisView.SelectedItem.Cast().As<UiComponentViewModel>();
        if (selectedComponent is null)
        {
            return;
        }
        _ = this.ViewModel.Components.Remove(selectedComponent);
        await this.BindAllComponentsView();
        this.Debug("Selected component removed.");
    }

    private void ResetBlazorPageCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        => e.CanExecute = this.IsEditMode && this._canReset;

    private async void ResetBlazorPageCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        this.ViewModel = null;
        this.IsEditMode = false;
        await this.BindPageTreeViewAsync();
        this.EndActionScope();
    }

    private void SaveBlazorPageCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        => e.CanExecute = this.IsEditMode;

    private async void SaveBlazorPageCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        this.ValidateForm();
        this.ViewModel = await this._service.SaveViewModelAsync(this.ViewModel).ThrowOnFailAsync(this.Title);
        this.RefreshPageComponents();
        this.Debug("Saved to database.");
    }

    private async Task<object?> SaveToDisk()
    {
        this.ValidateForm();
        var viewModel = this.ViewModel!;
        var code = this._service.GenerateCodes(viewModel, new(viewModel.GenerateMainCode, viewModel.GeneratePartialCode, viewModel.GenerateUiCode)).GetValue();
        _ = await code.SaveToFileAsync();
        return "Codes saved.";
    }

    private void SaveToDiskCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        => e.CanExecute = this.IsEditMode;

    private async void SaveToDiskCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        => await this.Logger.LogBlockAsync(this.SaveToDisk);

    private async void SaveToFileButton_Click(object sender, RoutedEventArgs e)
    {
        this.ValidateForm();
        var viewModel = this.ViewModel;
        var code = this._service.GenerateCodes(viewModel, new(viewModel.GenerateMainCode, viewModel.GeneratePartialCode, viewModel.GenerateUiCode)).GetValue();
        _ = await code.SaveToFileAsync();
        this.EndActionScope("Codes saved.");
    }

    private void SelectModuleUserControl_Initializing(object sender, InitialItemEventArgs<IModuleService> e)
        => e.Item = this._moduleService;

    private void ValidateCommandBinding_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        => e.CanExecute = this.IsEditMode;

    private void ValidateCommandBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        this.ValidateForm();
        _ = MsgBox2.Show("Form state is valid.");
    }

    [MemberNotNull(nameof(ViewModel))]
    private void ValidateForm()
    {
        //! It's kinda impossible. Just to shut VS up.
        Check.MustBeNotNull(this.ViewModel, () => new ValidationException("Please create a new Page or edit one."));
        _ = this._service.CheckValidator(this.ViewModel);
    }

    private void ViewModelComponents_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        => this.RefreshPageComponents();
}