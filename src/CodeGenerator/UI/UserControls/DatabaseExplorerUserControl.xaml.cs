using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

using Contracts.Services;

using HanyCo.Infra.UI.ViewModels;

using Library.Collections;
using Library.EventsArgs;
using Library.Threading.MultistepProgress;
using Library.Validations;

using static Library.Wpf.Helpers.ControlHelper;

namespace HanyCo.Infra.UI.UserControls;

[DefaultProperty(nameof(SelectedDbObjectNode))]
[DefaultEvent(nameof(SelectedDbObjectNodeChanged))]
public partial class DatabaseExplorerUserControl : UserControl, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public event EventHandler<ItemActedEventArgs<Node<DbObjectViewModel>?>>? SelectedDbObjectNodeChanged;

    public DatabaseExplorerUserControl()
        => this.InitializeComponent();

    #region Node<DbObjectViewModel> SelectedDbObjectNode

    public static readonly DependencyProperty SelectedDbObjectNodeProperty = GetDependencyProperty<Node<DbObjectViewModel>?, DatabaseExplorerUserControl>(
        nameof(SelectedDbObjectNode));

    public Node<DbObjectViewModel>? SelectedDbObjectNode
    {
        get => (Node<DbObjectViewModel>)this.GetValue(SelectedDbObjectNodeProperty);
        set => this.SetValue(SelectedDbObjectNodeProperty, value);
    }

    #endregion Node<DbObjectViewModel> SelectedDbObjectNode

    #region DbTableViewModel SelectedTable

    public static readonly DependencyProperty SelectedTableProperty = GetDependencyProperty<DbTableViewModel?, DatabaseExplorerUserControl>(
        nameof(SelectedTable));

    public DbTableViewModel? SelectedTable
    {
        get => (DbTableViewModel)this.GetValue(SelectedTableProperty);
        set => this.SetValue(SelectedTableProperty, value);
    }

    #endregion DbTableViewModel SelectedTable

    #region IEnumerable<DbColumnViewModel> SelectedColumns

    public static readonly DependencyProperty SelectedColumnsProperty = GetDependencyProperty<IEnumerable<DbColumnViewModel>, DatabaseExplorerUserControl>(
        nameof(SelectedColumns),
        onPropertyChanged: (me, e) => me.OnPropertyChanged(e),
        defaultValue: Enumerable.Empty<DbColumnViewModel>());

    public IEnumerable<DbColumnViewModel> SelectedColumns
    {
        get => (IEnumerable<DbColumnViewModel>)this.GetValue(SelectedColumnsProperty);
        set => this.SetValue(SelectedColumnsProperty, value);
    }

    #endregion IEnumerable<DbColumnViewModel> SelectedColumns

    public async Task<DatabaseExplorerUserControl> InitializeAsync(IDbTableService dbTableService, IProgressReport? reporter = null)
    {
        var dbNode = await dbTableService.NotNull().GetTablesTreeViewItemAsync(new(SettingsService.Load().connectionString!, reporter: reporter));
        var treeItems = new TreeViewItem { Header = "Tables" };
        EnumerableHelper.BuildTree<Node<DbObjectViewModel>, TreeViewItem>(
            dbNode,
            dbObject => new() { Header = dbObject.Value, DataContext = dbObject },
            dbObject => dbObject.Children,
            item => treeItems.Items.Add(item),
            (parent, child) => parent.Items.Add(child));
        _ = this.ServerExplorerTreeView.Items.ClearAndAdd(treeItems);
        return this;
    }

    private void OnPropertyChanged(string? propertyName)
    {
        if (propertyName.IsNullOrEmpty())
        {
            return;
        }

        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void ServerExplorerTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        this.SelectedDbObjectNode = this.ServerExplorerTreeView.GetSelectedValue<Node<DbObjectViewModel>>();

        (this.SelectedTable, this.SelectedColumns) = this.SelectedDbObjectNode?.Value switch
        {
            DbTableViewModel => onTableSelected(this.SelectedDbObjectNode),
            DbColumnViewModel => onColumnSelected(this.SelectedDbObjectNode),
            _ => onUnknownSelected(this.SelectedDbObjectNode),
        };
        SelectedDbObjectNodeChanged?.Invoke(this, new ItemActedEventArgs<Node<DbObjectViewModel>?>(this.SelectedDbObjectNode));
        return;

        static (DbTableViewModel? DbTable, IEnumerable<DbColumnViewModel> dbColumns) onTableSelected(Node<DbObjectViewModel>? node)
        {
            var dbTable = node?.Value.Cast().As<DbTableViewModel>();
            var children = node?.Children?.First()?.Children;
            var list = (children?.Select(x => x?.Value?.Cast().As<DbColumnViewModel>()).Compact() ?? Enumerable.Empty<DbColumnViewModel>()).ToList();
            var dbColumns = list;
            return (dbTable, dbColumns);
        }
        static (DbTableViewModel? DbTable, IEnumerable<DbColumnViewModel> dbColumns) onColumnSelected(Node<DbObjectViewModel>? node) 
            => (null, EnumerableHelper.ToEnumerable(node?.Value.Cast().As<DbColumnViewModel>()).Compact() ?? Enumerable.Empty<DbColumnViewModel>());

        static (DbTableViewModel? DbTable, IEnumerable<DbColumnViewModel> dbColumns) onUnknownSelected(Node<DbObjectViewModel>? node) 
            => (null, Enumerable.Empty<DbColumnViewModel>());
    }
}