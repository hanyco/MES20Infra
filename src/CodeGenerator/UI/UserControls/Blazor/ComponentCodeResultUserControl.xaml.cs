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
    private Codes? _codes;

    public ComponentCodeResultUserControl() =>
        this.InitializeComponent();

    public Codes? Codes
    {
        get => this._codes;
        set
        {
            this._codes = value;
            this.OnCodesChanged();
        }
    }

    private void CodeNamesTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        var code = e.NewValue.As<TreeViewItem>()?.DataContext.As<Code>();
        this.CodeStatementContentControl.Content =
            code == null
                ? null
                : new CodeDetailsUserControl { DataContext = code };
    }

    private void OnCodesChanged() =>
        this.CodeNamesTreeView.BindItems(this.Codes);
}