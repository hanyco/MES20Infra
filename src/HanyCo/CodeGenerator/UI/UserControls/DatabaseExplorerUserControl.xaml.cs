using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

using HanyCo.Infra.CodeGen.Domain.Services;
using HanyCo.Infra.CodeGen.Domain.ViewModels;

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
        defaultValue: []);

    public IEnumerable<DbColumnViewModel> SelectedColumns
    {
        get => (IEnumerable<DbColumnViewModel>)this.GetValue(SelectedColumnsProperty);
        set => this.SetValue(SelectedColumnsProperty, value);
    }

    #endregion IEnumerable<DbColumnViewModel> SelectedColumns

    public async Task<DatabaseExplorerUserControl> InitializeAsync(IDbTableService dbTableService, IProgressReport? reporter = null)
    {
        var dbNode = await dbTableService.NotNull().GetTablesTreeViewItemAsync(new(SettingsService.Get().connectionString!, reporter: reporter));
        var treeItems = new TreeViewItem { Header = "Tables" };
        EnumerableHelper.BuildTree<Node<DbObjectViewModel>, TreeViewItem>(
            dbNode,
            dbObject => new() { Header = dbObject.Value, DataContext = dbObject, Name = dbObject.Value?.Name },
            dbObject => dbObject.Children,
            item => treeItems.Items.Add(item),
            (parent, child) => parent.Items.Add(child));
        _ = this.TreeView.Items.ClearAndAdd(treeItems);
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
        this.SelectedDbObjectNode = this.TreeView.GetSelectedModel<Node<DbObjectViewModel>>();

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
            if (node?.Value == null)
            {
                return default;
            }

            var dbTable = node.Value.Cast().As<DbTableViewModel>();
            var children = node.Children?.FirstOrDefault()?.Children;
            var dbColumns = (children?.Select(x => x?.Value?.Cast().As<DbColumnViewModel>()).Compact() ?? []).ToList();
            return (dbTable, dbColumns);
        }
        static (DbTableViewModel? DbTable, IEnumerable<DbColumnViewModel> dbColumns) onColumnSelected(Node<DbObjectViewModel>? node) =>
            (null, EnumerableHelper.AsEnumerable(node?.Value.Cast().As<DbColumnViewModel>()).Compact() ?? []);

        static (DbTableViewModel? DbTable, IEnumerable<DbColumnViewModel> dbColumns) onUnknownSelected(Node<DbObjectViewModel>? node) =>
            (null, []);
    }

    private void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        this.TreeView.FilterItems(
            this.FilterTextBox.Text,
            item => item.GetModel<Node<DbObjectViewModel>>()?.ToString(),
            this.TreeView.Items[0].Cast().To<TreeViewItem>().Items);
    }

    private void ClearFilterButton_Click(object sender, RoutedEventArgs e)
    {
        this.FilterTextBox.Text = string.Empty;
    }
}