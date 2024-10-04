using System.Windows;
using System.Windows.Controls;

using HanyCo.Infra.CodeGen.Domain.Services;
using HanyCo.Infra.CodeGen.Domain.ViewModels;

using Library.ComponentModel;

namespace HanyCo.Infra.UI.UserControls;

/// <summary>
/// Interaction logic for FunctionalityTreeView.xaml
/// </summary>
public partial class FunctionalityTreeView : UserControl, IBinable<IEnumerable<FunctionalityViewModel>>, IAsyncBindable
{
    public static readonly DependencyProperty SelectedItemProperty =
        ControlHelper.GetDependencyProperty<FunctionalityViewModel?, FunctionalityTreeView>(nameof(SelectedItem));

    private IFunctionalityService? _service;

    public FunctionalityTreeView() => this.InitializeComponent();

    public FunctionalityViewModel? SelectedItem
    {
        get => (FunctionalityViewModel?)this.GetValue(SelectedItemProperty);
        set => this.SetValue(SelectedItemProperty, value);
    }

    public FunctionalityViewModel? ViewModel { get; set; }

    public void Bind(IEnumerable<FunctionalityViewModel> viewMode) =>
        this.TreeView.BindItems(viewMode);

    public async Task BindAsync()
    {
        this._service ??= DI.GetService<IFunctionalityService>();
        var functionalities = await this._service.GetAllAsync();
        this.Bind(functionalities);
    }

    public void Rebind() =>
        this.TreeView.RebindItemsSource();

    private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) =>
        this.SelectedItem = e.GetModel<FunctionalityViewModel>();

    private void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        this.TreeView.FilterItems(
            this.FilterTextBox.Text,
            item => item.GetModel<InfraViewModelBase>()?.Name,
            this.TreeView.Items[0].Cast().To<TreeViewItem>().Items);
    }

    private void ClearFilterButton_Click(object sender, RoutedEventArgs e)
    {
        this.FilterTextBox.Text = string.Empty;
    }
}