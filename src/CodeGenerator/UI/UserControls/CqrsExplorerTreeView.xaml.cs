using System.Collections.Immutable;
using System.Windows;
using System.Windows.Controls;

using Library.EventsArgs;

namespace HanyCo.Infra.UI.UserControls;

public partial class CqrsExplorerTreeView : UserControl
{
    private readonly ICqrsCommandService _commandService;
    private readonly IDtoService _dtoService;
    private readonly ICqrsQueryService _queryService;

    private ImmutableArray<TreeViewItem> _result;

    public event EventHandler<ItemActingEventArgs<Task<IReadOnlyList<CqrsCommandViewModel>>>>? GettingCommands;

    public event EventHandler<ItemActingEventArgs<Task<IReadOnlyList<DtoViewModel>>>>? GettingDtos;

    public event EventHandler<ItemActingEventArgs<Task<IReadOnlyList<CqrsQueryViewModel>>>>? GettingQueries;

    public event EventHandler<ItemActedEventArgs<InfraViewModelBase>>? ItemDoubleClicked;

    public event EventHandler<ItemActedEventArgs<InfraViewModelBase?>>? SelectedItemChanged;

    public CqrsExplorerTreeView()
    {
        this.InitializeComponent();
        if (ControlHelper.IsDesignTime())
        {
            return;
        }

        this.BeginInitializing();
        this._dtoService = DI.GetService<IDtoService>()!;
        this._queryService = DI.GetService<ICqrsQueryService>()!;
        this._commandService = DI.GetService<ICqrsCommandService>()!;
    }

    public static DependencyProperty FilterDtoParamsProperty { get; } = ControlHelper.GetDependencyProperty<bool?, CqrsExplorerTreeView>(nameof(FilterDtoParams), onPropertyChanged: async (me, _) => await me.BindAsync(), defaultValue: null);
    public static DependencyProperty FilterResultDtoProperty { get; } = ControlHelper.GetDependencyProperty<bool?, CqrsExplorerTreeView>(nameof(FilterDtoResult), onPropertyChanged: async (me, _) => await me.BindAsync(), defaultValue: null);
    public static DependencyProperty FilterViewModelProperty { get; } = ControlHelper.GetDependencyProperty<bool?, CqrsExplorerTreeView>(nameof(FilterViewModel), onPropertyChanged: async (me, _) => await me.BindAsync(), defaultValue: null);

    public static DependencyProperty LoadCommandsProperty { get; } = ControlHelper.GetDependencyProperty<bool, CqrsExplorerTreeView>(nameof(LoadCommands), onPropertyChanged: async (me, _) => await me.BindAsync());
    public static DependencyProperty LoadDtosProperty { get; } = ControlHelper.GetDependencyProperty<bool, CqrsExplorerTreeView>(nameof(LoadDtos), onPropertyChanged: async (me, _) => await me.BindAsync());
    public static DependencyProperty LoadQueriesProperty { get; } = ControlHelper.GetDependencyProperty<bool, CqrsExplorerTreeView>(nameof(LoadQueries), onPropertyChanged: async (me, _) => await me.BindAsync());

    public static DependencyProperty SelectedItemProperty { get; } = ControlHelper.GetDependencyProperty<InfraViewModelBase?, CqrsExplorerTreeView>(propertyName: nameof(SelectedItem), onPropertyChanged: (me, _) => me.OnSelectedItemChanged());

    public bool? FilterDtoParams { get => (bool?)this.GetValue(FilterDtoParamsProperty); set => this.SetValue(FilterDtoParamsProperty, value); }
    public bool? FilterDtoResult { get => (bool?)this.GetValue(FilterResultDtoProperty); set => this.SetValue(FilterResultDtoProperty, value); }
    public bool? FilterViewModel { get => (bool?)this.GetValue(FilterViewModelProperty); set => this.SetValue(FilterViewModelProperty, value); }
    public bool IsInitializing { get; private set; }
    public bool LoadCommands { get => (bool)this.GetValue(LoadCommandsProperty); set => this.SetValue(LoadCommandsProperty, value); }
    public bool LoadDtos { get => (bool)this.GetValue(LoadDtosProperty); set => this.SetValue(LoadDtosProperty, value); }
    public bool LoadQueries { get => (bool)this.GetValue(LoadQueriesProperty); set => this.SetValue(LoadQueriesProperty, value); }
    public InfraViewModelBase? SelectedItem { get => (InfraViewModelBase)this.GetValue(SelectedItemProperty); set => this.SetValue(SelectedItemProperty, value); }

    public async Task BindAsync()
    {
        if (this.IsInitializing)
        {
            return;
        }

        if (this._dtoService is null || this._queryService is null || this._commandService is null)
        {
            // Still in design-mode.
            return;
        }

        var result = new List<TreeViewItem>();

        if (this.LoadDtos)
        {
            var dtosTreeViewItemRoot = ControlHelper.BindNewTreeViewItem(InfraViewModelBase.NewEmpty("Models"));
            var dtos = await this.OnGetDtosAsync();
            result.Add(ControlHelper.BindNewTreeViewItems(dtosTreeViewItemRoot, dtos));
            dtosTreeViewItemRoot.IsExpanded = true;
        }

        if (this.LoadQueries)
        {
            var queriesTreeViewItemRoot = ControlHelper.BindNewTreeViewItem(InfraViewModelBase.NewEmpty("Queries"));
            var queries = await this.OnGetQueriesAsync();
            result.Add(ControlHelper.BindNewTreeViewItems(queriesTreeViewItemRoot, queries));
            queriesTreeViewItemRoot.IsExpanded = true;
        }

        if (this.LoadCommands)
        {
            var commandsViewItemRoot = ControlHelper.BindNewTreeViewItem(InfraViewModelBase.NewEmpty("Commands"));
            var commands = await this.OnGetCommandsAsync();
            result.Add(ControlHelper.BindNewTreeViewItems(commandsViewItemRoot, commands));
            commandsViewItemRoot.IsExpanded = true;
        }
        this._result = [.. result];
        _ = this.TreeView.BindItemsSource(result);
    }

    public void EndInitializing() =>
        this.IsInitializing = false;

    public async Task ReloadCqrsExplorerAsync(bool loadDtos, bool loadQueries, bool loadCommands)
    {
        this.BeginInitializing();
        this.LoadDtos = loadDtos;
        this.LoadQueries = loadQueries;
        this.LoadCommands = loadCommands;
        this.EndInitializing();
        await this.RebindAsync();
    }

    private void BeginInitializing() =>
        this.IsInitializing = true;

    private void ClearFilterButton_Click(object sender, RoutedEventArgs e) =>
        this.FilterTextBox.Text = string.Empty;

    private void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        => this.TreeView.FilterTreeView(
            this.FilterTextBox.Text,
            item => item.GetModel<InfraViewModelBase>()?.Name,
            this.TreeView.Items[0].Cast().To<TreeViewItem>().Items);

    private async Task<IEnumerable<CqrsCommandViewModel>> OnGetCommandsAsync()
    {
        if (this.GettingCommands is null)
        {
            return await this._commandService.GetAllAsync();
        }

        var e = new ItemActingEventArgs<Task<IReadOnlyList<CqrsCommandViewModel>>>();
        GettingCommands(this, e);
        return e.Handled ? await e.Item : await this._commandService.GetAllAsync();
    }

    private async Task<IEnumerable<DtoViewModel>> OnGetDtosAsync()
    {
        if (this.GettingDtos is null)
        {
            return await this._dtoService.GetAllByCategoryAsync(this.FilterDtoParams, this.FilterDtoResult, this.FilterViewModel);
        }

        var e = new ItemActingEventArgs<Task<IReadOnlyList<DtoViewModel>>>();
        GettingDtos(this, e);
        return e.Handled ? await e.Item : await this._dtoService.GetAllAsync();
    }

    private async Task<IEnumerable<CqrsQueryViewModel>> OnGetQueriesAsync()
    {
        if (this.GettingQueries is null)
        {
            return await this._queryService.GetAllAsync();
        }

        var e = new ItemActingEventArgs<Task<IReadOnlyList<CqrsQueryViewModel>>>();
        GettingQueries(this, e);
        return e.Handled ? await e.Item : await this._queryService.GetAllAsync();
    }

    private void OnItemDoubleClicked(ItemActedEventArgs<InfraViewModelBase> e) =>
        this.ItemDoubleClicked?.Invoke(this, e);

    private void OnSelectedItemChanged() =>
        SelectedItemChanged?.Invoke(this, new ItemActedEventArgs<InfraViewModelBase?>(this.SelectedItem));

    private async Task RebindAsync() =>
        await this.BindAsync();

    private void TreeView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (ControlHelper.GetSelectedModel<InfraViewModelBase>(this.TreeView) is { } selectedItem)
        {
            this.SelectedItem = selectedItem;
            this.OnItemDoubleClicked(new(selectedItem));
        }
    }

    private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) =>
        this.SelectedItem = ControlHelper.GetSelectedModel<InfraViewModelBase>(this.TreeView);

    private async void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        this.EndInitializing();
        await this.RebindAsync();
    }
}