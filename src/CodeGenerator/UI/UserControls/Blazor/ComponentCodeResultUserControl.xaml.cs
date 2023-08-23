using System.Windows;
using System.Windows.Controls;

using HanyCo.Infra.UI.UserControls.Blazor;

using Library.CodeGeneration.Models;

namespace HanyCo.Infra.UI.Pages.Blazor;

/// <summary>
/// Interaction logic for ComponentCodeResultUserControl.xaml
/// </summary>
public partial class ComponentCodeResultUserControl
{
    // Using a DependencyProperty as the backing store for Codes. This enables animation, styling,
    // binding, etc...
    public static readonly DependencyProperty CodesProperty = ControlHelper.GetDependencyProperty<Codes?, ComponentCodeResultUserControl>(nameof(Codes), onPropertyChanged: (me, _) => me.OnCodesChanged());

    public ComponentCodeResultUserControl() =>
        this.InitializeComponent();

    public Codes? Codes
    {
        get => (Codes?)this.GetValue(CodesProperty);
        set => this.SetValue(CodesProperty, value);
    }

    private void CodeNamesTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        var code = e.NewValue.Cast().As<TreeViewItem>()?.DataContext.Cast().As<Code>();
        this.CodeStatementContentControl.Content =
            code == null
                ? null
                : new CodeDetailsUserControl { DataContext = code };
    }

    private void OnCodesChanged() =>
        this.CodeNamesTreeView.BindItems(this.Codes);
}