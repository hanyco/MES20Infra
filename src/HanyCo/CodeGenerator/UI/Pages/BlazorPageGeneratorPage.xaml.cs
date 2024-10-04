using System.Diagnostics.CodeAnalysis;
using System.Windows;

using HanyCo.Infra.CodeGen.Domain.ViewModels;
using HanyCo.Infra.UI.Dialogs;
using HanyCo.Infra.UI.Helpers;

using Library.Data.EntityFrameworkCore;
using Library.EventsArgs;
using Library.Exceptions.Validations;
using Library.Validations;
using Library.Wpf.Dialogs;

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

    public static readonly DependencyProperty IsEditModeProperty
        = ControlHelper.GetDependencyProperty<bool, BlazorPageGeneratorPage>(nameof(IsEditMode), onPropertyChanged: (me, _) =>
        {
            me.CreatePageButton.IsEnabled
            = me.EditPageButton.IsEnabled
            = me.DeletePageButton.IsEnabled
            = !me.IsEditMode;
            me.SaveToDbButton.IsEnabled
            = me.GenerateCodeButton.IsEnabled
            = me.SaveToDiskButton.IsEnabled
            = me.ResetButton.IsEnabled
            = me.IsEditMode;
        });

    public static readonly DependencyProperty SelectedComponentInAllProperty =
        ControlHelper.GetDependencyProperty<UiComponentViewModel, BlazorPageGeneratorPage>(nameof(SelectedComponentInAll));

    private readonly IBlazorPageCodeService _codingService;
    private readonly IBlazorComponentService _componentService;
    private readonly IModuleService _moduleService;
    private readonly IBlazorPageService _service;

    #endregion Fields

    public BlazorPageGeneratorPage(IBlazorPageService service, IModuleService moduleService, IBlazorComponentService componentService, ILogger logger, IBlazorPageCodeService codingService)
        : base(logger)
    {
        this._service = service;
        this._moduleService = moduleService;
        this._componentService = componentService;

        this.InitializeComponent();
        this._codingService = codingService;
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
        var scope = this.ActionScopeBegin();
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
        if (this.ViewModel?.DataContext?.Id is not { } dtoId)
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
        if (!SelectCqrsDialog.Show<DtoViewModel>(out var result, new("Select ViewModel", SelectCqrsDialog.LoadEntity.Dto)) || result is not { } dto)
        {
            return;
        }

        this.ViewModel = this._service.CreateViewModel(dto);
        await this.BindAllComponentsView();
        this.IsEditMode = true;
        this.EndActionScope();
    }

    private async void CreatePageButton_Click(object sender, RoutedEventArgs e) =>
        await this.Logger.LogBlockAsync(this.CreatePage);

    private async Task DeletePage()
    {
        var page = ControlHelper.GetSelectedModel<UiPageViewModel>(this.PageTreeView);
        Check.MustBeNotNull(page, () => new ValidationException("Please select a page to edit."));
        if (MsgBox2.AskWithWarn("Are you sure you want to delete selected component?") != TaskDialogResult.Yes)
        {
            return;
        }

        var deleteResult = await this._service.DeleteAsync(page).ThrowOnFailAsync(this.Title);
        this.IsEditMode = false;
        await this.BindPageTreeViewAsync();
        _ = this.EndActionScope(deleteResult);
    }

    private async void DeletePageButton_Click(object sender, RoutedEventArgs e) =>
        await this.DeletePage();

    private async Task EditPage()
    {
        if (this.IsEditMode)
        {
            return;
        }

        var scope = this.ActionScopeBegin("Loading...");
        var id = ControlHelper.GetSelectedModel<UiPageViewModel>(this.PageTreeView)?.Id;
        Check.MustBeNotNull(id, () => "Please select a page to edit.");
        this.ViewModel = await this._service.GetByIdAsync(id.Value);
        await this.BindAllComponentsView();
        this.IsEditMode = true;
        scope.End();
    }

    private async void EditPageButton_Click(object sender, RoutedEventArgs e) =>
        await this.EditPage();

    private void ElementPositionUserControl_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
    }

    private Task GenerateCodeAsync()
    {
        var scope = this.ActionScopeBegin("Generating code…");
        var model = this.ViewModel!;
        _ = this._service.Validate(model).ThrowOnFail();
        this.ComponentCodeResultUserControl.Codes = this._codingService.GenerateCodes(model, new(model.GenerateMainCode, model.GeneratePartialCode, model.GenerateUiCode));
        this.ResultsTabItem.IsSelected = true;
        this.Debug("Code generated.");
        scope.End();
        return Task.CompletedTask;
    }

    private async void GenerateCodeButton_Click(object sender, RoutedEventArgs e) => 
        await this.GenerateCodeAsync();

    private async void Page_BindingData(object sender, EventArgs e)
    {
        await this.BindPageTreeViewAsync();
        this.EndActionScope();
    }

    private void PageComponentLisView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
    }

    private async void PageTreeView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        => await this.EditPage();

    private void PageTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    { }

    private void RefreshPageComponents()
    {
        if (this.ViewModel?.Components?.Any() is not true)
        {
            return;
        }

        _ = this.PageComponentLisView.BindItemsSource(this.ViewModel.Components);
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

    private async void ResetButton_Click(object sender, RoutedEventArgs e)
    {
        this.ViewModel = null;
        this.IsEditMode = false;
        await this.BindPageTreeViewAsync();
        this.EndActionScope();
    }

    private async void SaveToDbButton_Click(object sender, RoutedEventArgs e)
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
        var code = this._codingService.GenerateCodes(viewModel, new(viewModel.GenerateMainCode, viewModel.GeneratePartialCode, viewModel.GenerateUiCode)).Value;
        _ = await code.SaveToFileAskAsync();
        return "Codes saved.";
    }

    private async void SaveToDiskButton_Click(object sender, RoutedEventArgs e) => await this.Logger.LogBlockAsync(this.SaveToDisk);

    private async void SaveToFileButton_Click(object sender, RoutedEventArgs e)
    {
        this.ValidateForm();
        var viewModel = this.ViewModel;
        var code = this._codingService.GenerateCodes(viewModel, new(viewModel.GenerateMainCode, viewModel.GeneratePartialCode, viewModel.GenerateUiCode)).Value;
        _ = await code.SaveToFileAskAsync();
        this.EndActionScope("Codes saved.");
    }

    private void SelectModuleUserControl_Initializing(object sender, InitialItemEventArgs<IModuleService> e)
        => e.Item = this._moduleService;

    [MemberNotNull(nameof(ViewModel))]
    private void ValidateForm() 
        => this._service.Validate(this.ViewModel!);

    private void ViewModelComponents_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        => this.RefreshPageComponents();

    private void Me_Loaded(object sender, RoutedEventArgs e)
    {
        this.IsEditMode = true;
        this.IsEditMode = false;
    }
}