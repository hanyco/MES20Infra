using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;




using HanyCo.Infra.Exceptions;
using HanyCo.Infra.UI.Helpers;
using HanyCo.Infra.UI.Services;
using HanyCo.Infra.UI.UserControls;


using Library.BusinessServices;
using Library.Collections;
using Library.EventsArgs;
using Library.Exceptions.Validations;
using Library.Results;
using Library.Threading.MultistepProgress;
using Library.Validations;
using Library.Wpf.Dialogs;

using Microsoft.EntityFrameworkCore;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace HanyCo.Infra.UI.Controls.Pages;

/// <summary>
/// Interaction logic for DtoDetailsPage.xaml
/// </summary>
public partial class DtoDetailsPage
{
    private readonly IDtoCodeService _codeService;
    private readonly IDbTableService _dbTableService;
    private readonly IProgressReport _reporter;
    private readonly IDtoService _service;
    private DatabaseExplorerUserControl? _databaseExplorerUserControl;

    public DtoDetailsPage(
        IDtoService dtoService,
        IDtoCodeService codeService,
        IDbTableService dbTableService,
        IPropertyService propertyService,
        IProgressReport reporter,
        ILogger logger)
        : base(logger)
    {
        this.DataContextChanged += this.DtoDetailsPage_DataContextChanged;
        this._service = dtoService;
        this._codeService = codeService;
        this._dbTableService = dbTableService;
        this._reporter = reporter;

        this.InitializeComponent();
        this.DtoEditUserControl.Initialize();
    }

    public DtoViewModel? ViewModel
    {
        get => this.DataContext.Cast().As<DtoViewModel>();
        set
        {
            if (!value?.Equals(this.DataContext) ?? this.DataContext is not null)
            {
                this.DataContext = value;
                value?.DeletedProperties.Clear();
            }
        }
    }

    private void AddColumnToDto(in DbColumnViewModel column)
        => this.ViewModel.NotNull().Properties.Add(new()
        {
            IsNullable = column.IsNullable,
            Name = column.Name,
            DbObject = new DbColumnViewModel(column.Name.NotNull(), column.ObjectId, column.DbType, column.IsNullable, column.MaxLength),
        });

    private void AddColumnToDtoButton_Click(object sender, RoutedEventArgs e)
    {
        var node = this._databaseExplorerUserControl?.SelectedDbObjectNode;
        var dbObject = node?.Value;

        if (dbObject is DbColumnViewModel column)
        {
            this.AddColumnToDto(column);
        }
        else if (dbObject is DbTableViewModel)
        {
            var columns = node!.Children.Select(n => n.Value).Cast<DbColumnViewModel>();
            foreach (var col in columns)
            {
                this.AddColumnToDto(col);
            }
        }
        else
        {
            throw new ValidationException("Please select a table column", "No column is selected", this.Title);
        }
    }

    private void ContextMenu_ContextMenuOpening(object sender, ContextMenuEventArgs e)
    {
    }

    private void CqrsExplorerTreeView_ItemDoubleClicked(object sender, ItemActedEventArgs<InfraViewModelBase> e)
        => this.EditDtoButton.IsEnabled.IfTrue(this.EditDtoButton.PerformClick);

    private void CqrsExplorerTreeView_SelectedItemChanged(object sender, ItemActedEventArgs<InfraViewModelBase> e)
        => this.RefreshFormState();

    private async void CreateDtoWithTableMenuItem_Click(object sender, RoutedEventArgs e)
    {
        _ = this.ActionScopeBegin();
        this.IsEnabled = false;
        try
        {
            if (this._databaseExplorerUserControl == null)
            {
                this._databaseExplorerUserControl = new();
                _ = await this._databaseExplorerUserControl.InitializeAsync(this._dbTableService, this._reporter);
            }
            _ = HostDialog.ShowDialog(this._databaseExplorerUserControl, "Select Table", "Select a Table to create a DTO.", _ => Check.If(this._databaseExplorerUserControl.SelectedTable is null, () => "Please select a Table.")).BreakOnFail();
            var tableNode = this._databaseExplorerUserControl.SelectedDbObjectNode;
            Check.MustBeNotNull(tableNode, () => "Please select a table");

            var columns = await this._dbTableService.GetColumnsAsync(SettingsService.Get().connectionString!, tableNode.Value.Name!);
            this.ViewModel = this._service.CreateByDbTable(DbTableViewModel.FromDbObjectViewModel(tableNode!), columns.Compact());
            this.EndActionScope();
            this.RefreshFormState();
        }
        finally
        {
            this.IsEnabled = true;
        }
    }

    private void DatabaseExplorerUserControl_SelectedDbObjectNodeChanged(object sender, ItemActedEventArgs<Node<DbObjectViewModel>> e)
        => (this.NewDtoFromTableButton.IsEnabled, this.AddToDtoButton.IsEnabled) = (e.Item?.Value is DbTableViewModel, e.Item?.Value is DbColumnViewModel);

    private async void DeleteDtoButton_Click(object sender, RoutedEventArgs e)
    {
        var dto = this.CqrsExplorerTreeView.SelectedItem.Cast().As<DtoViewModel>();
        Check.MustBeNotNull(dto, () => new ValidationException("Please select a DTO"));

        if (MsgBox2.AskWithWarn(
            $"Deleting DTO '{dto.Name}'.",
            "Are you sure you want to permanently delete it?",
            footerIcon: TaskDialogStandardIcon.Information,
            detailsCollapsedLabel: "More &Details",
            detailsExpandedText: "This DTO would be unable to delete, if it has reference to any CQRS segregate.") != TaskDialogResult.Yes)
        {
            return;
        }
        Application.Current.DoEvents();
        this.Debug("DTO deleting...");
        var saveResult = await this._service.DeleteAsync(dto).ConfigureAwait(false);
        if (saveResult.IsFailure && saveResult.Status is DbUpdateException ex)
        {
            if (ex.InnerException?.Message.Contains("infra.CqrsSegregate") ?? false)
            {
                MesException.Throw("A CQRS segregation has relation to this DTO. Therefore this DTO cannot be deleted.");
            }
        }
        _ = saveResult.ThrowOnFail();
        await this.InitDtoExplorerTreeAsync().ConfigureAwait(false);
        this.Debug("DTO deleted.");
    }

    private async void DtoDetailsPage_Binding(object sender, EventArgs e)
    {
        var scope = this.ActionScopeBegin("Initializing... Please wait.");
        await this.InitDtoExplorerTreeAsync().ConfigureAwait(false);
        await this.DtoEditUserControl.BindAsync().ConfigureAwait(false);
        this.RefreshFormState();

        scope.End();
    }

    private void DtoDetailsPage_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) =>
        this.RefreshFormState();

    private void DtoExplorerTreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (this.AddToDtoButton.IsEnabled)
        {
            this.AddToDtoButton.PerformClick();
        }
    }

    private void DtoExplorerTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        var viewModel = e.NewValue.Cast().As<TreeViewItem>()?.DataContext.Cast().As<InfraViewModelBase>();
        this.DeleteDtoButton.IsEnabled = viewModel is DtoViewModel;
        this.EditDtoButton.IsEnabled = viewModel is DtoViewModel;
    }

    private async void EditDtoButton_Click(object sender, RoutedEventArgs e)
    {
        var dto = this.CqrsExplorerTreeView.SelectedItem.Cast().As<DtoViewModel>();
        Check.MustBeNotNull(dto, () => "Please select a DTO");
        var viewModel = await this._service.GetByIdAsync(dto.Id.NotNull().Value).ConfigureAwait(false);
        this.ViewModel = viewModel.NotNull(() => new NotFoundValidationException("Entity not found."));
        this.EndActionScope();
    }

    private void GenerateCodeButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            this.Debug("Generating code...");
            this.GenerateCodeButton.IsEnabled = false;
            this.ComponentCodeResultUserControl.Codes = this._codeService.GenerateCodes(this.ViewModel!).ThrowOnFail(this.Title);
            this.DtoDetails.SelectedItem = this.ResultsTabItem;
            this.Debug("Code generated.");
        }
        finally
        {
            this.GenerateCodeButton.IsEnabled = true;
        }
    }

    private Task InitDtoExplorerTreeAsync()
        => this.CqrsExplorerTreeView.BindAsync();

    private void NewDtoButton_Click(object sender, RoutedEventArgs e)
    {
        this.ViewModel = new DtoViewModel();// { Id = --this._dtoId };
        this.EndActionScope();
    }

    private async void RefreshDatabaseButton_Click(object sender, RoutedEventArgs e)
    {
        await this.RebindDataAsync().ConfigureAwait(false);
        this.EndActionScope();
    }

    /// <summary>
    /// Refreshes the state of the form.
    /// </summary>
    private void RefreshFormState()
    {
        this.NewDtoFromTableButton.IsEnabled =
        this.NewDtoButton.IsEnabled =
        this.EditDtoButton.IsEnabled =
        this.DeleteDtoButton.IsEnabled =
        this.AddToDtoButton.IsEnabled =
        this.SaveDtoButton.IsEnabled =
        this.ResetFormButton.IsEnabled =
        this.DtoDetails.IsEnabled =
        this.GenerateCodeButton.IsEnabled =
        this.SaveCodeButton.IsEnabled =
        this.SecurityDescriptorButton.IsEnabled =
        this.AddToDtoButton.IsEnabled =
        this.CqrsExplorerTreeView.IsEnabled =
            false;
        this.DtoEditUserControl.RefreshState(this.ViewModel);
        if (this.ViewModel is null)
        {
            this.NewDtoFromTableButton.IsEnabled = true;
            this.NewDtoButton.IsEnabled = true;
            this.EditDtoButton.IsEnabled = true;
            this.DeleteDtoButton.IsEnabled = true;

            this.ComponentCodeResultUserControl.Codes = null;
            this.CqrsExplorerTreeView.IsEnabled = true;
        }
        else
        {
            this.AddToDtoButton.IsEnabled = true;
            this.SaveDtoButton.IsEnabled = true;
            this.ResetFormButton.IsEnabled = true;
            this.DtoDetails.IsEnabled = true;
            this.GenerateCodeButton.IsEnabled = true;
            this.SaveCodeButton.IsEnabled = true;
            this.SecurityDescriptorButton.IsEnabled = true;
            this.AddToDtoButton.IsEnabled = true;
        }
    }

    private void ResetFormButton_Click(object sender, RoutedEventArgs e)
    {
        this.ViewModel = null;
        this.RefreshFormState();
        this._service.ResetChanges();
        this.EndActionScope();
    }

    private async void SaveCodeButton_Click(object sender, RoutedEventArgs e)
    {
        if (this.ViewModel is null)
        {
            return;
        }

        var result = await saveSourceToDiskAskAsync().ThrowOnFailAsync(this.Title).ConfigureAwait(false);
        _ = this.EndActionScope(result);

        async Task<Result<string?>> saveSourceToDiskAskAsync()
        {
            _ = await this.ValidateFormAsync();

            var codes = this._codeService.GenerateCodes(this.ViewModel).Value;
            var result = await codes.SaveToFileAskAsync();
            return result;
        }
    }

    private async void SaveDtoButton_Click(object sender, RoutedEventArgs e)
    {
        _ = await this.ValidateFormAsync().ThrowOnFailAsync(this.Title).ConfigureAwait(false);

        this.SaveDtoButton.IsEnabled = false;
        try
        {
            await Lock(this, save).ConfigureAwait(false);
        }
        finally
        {
            this.SaveDtoButton.IsEnabled = true;
        }

        async Task save()
        {
            _ = this.DtoEditUserControl.Focus();

            this.Debug("Saving DTO…");
            var viewModel = this.ViewModel!;
            var saveResult = await this._service.SaveViewModelAsync(viewModel).ThrowOnFailAsync(this.Title).ConfigureAwait(false);

            this.Debug("Reloading DTO…");
            var resultViewModel = await this._service.GetByIdAsync(viewModel.Id!.Value).WaitAsync(TimeSpan.FromSeconds(15)).ConfigureAwait(false);
            this.RebindDataContext(resultViewModel);

            await this.InitDtoExplorerTreeAsync().ConfigureAwait(false);
            this.Debug("DTO saved.");
            _ = saveResult.ShowOrThrow();
        }
    }

    private void SecurityDescriptorButton_Click(object sender, RoutedEventArgs e)
    {
    }
}