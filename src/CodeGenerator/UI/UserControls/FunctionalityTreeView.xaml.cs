using System.Windows;
using System.Windows.Controls;

using Library.ComponentModel;

namespace HanyCo.Infra.UI.UserControls;

/// <summary>
/// Interaction logic for FunctionalityTreeView.xaml
/// </summary>
public partial class FunctionalityTreeView : UserControl, IBinable<IEnumerable<FunctionalityViewModel>>, IAsyncBindable
{
    public static readonly DependencyProperty SelectedItemProperty =
        ControlHelper.GetDependencyProperty<FunctionalityViewModel?, FunctionalityTreeView>(nameof(SelectedItem));

    private readonly IFunctionalityService _service = default!;

    public FunctionalityTreeView()
    {
        this.InitializeComponent();
        if (ControlHelper.IsDesignTime())
        {
            return;
        }
        this._service = DI.GetService<IFunctionalityService>();
    }

    public FunctionalityViewModel? SelectedItem
    {
        get => (FunctionalityViewModel?)this.GetValue(SelectedItemProperty);
        set => this.SetValue(SelectedItemProperty, value);
    }
    public FunctionalityViewModel ViewModel { get; set; }

    public void Bind(IEnumerable<FunctionalityViewModel> viewMode) =>
        ControlHelper.BindItemsSource(this.TreeView, viewMode);

    public async Task BindAsync()
    {
        var functionalities = await this._service.GetAllAsync();
        this.Bind(functionalities);
    }

    public void Rebind() => ControlHelper.RebindItemsSource(this.TreeView);
}