using System.Windows;
using System.Windows.Controls;

using HanyCo.Infra.UI.UserControls.Blazor;

using Library.CodeGeneration.Models;

using WinRT;

namespace HanyCo.Infra.UI.Pages.Blazor;

/// <summary>
/// Interaction logic for ComponentCodeResultUserControl.xaml
/// </summary>
public partial class ComponentCodeResultUserControl
{
    private Codes? _Codes;

    public ComponentCodeResultUserControl()
    {
        this.InitializeComponent();
        DataContextChanged += this.ComponentCodeResultUserControl_DataContextChanged;
    }

    public Codes? Codes
    {
        get => this._Codes;
        internal set
        {
            this._Codes = value;
            this.OnCodesChanged();
        }
    }

    private void CodeNamesTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        var code = e.NewValue.As<TreeViewItem>()?.DataContext.As<Code>();
        this.CodeStatementContentControl.Content =
            code == null
                ? null
                : new CodeDetailsUserControl() { DataContext = code };
    }

    private void ComponentCodeResultUserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        //this.DataContext.As<>()
    }

    private void OnCodesChanged() =>
        //this.CodesTabConrol.Items.Clear();
        //if (this.Codes is not null)
        //{
        //    foreach (var code in this.Codes)
        //    {
        //        _ = this.CodesTabConrol.Items.Add(new TabItem
        //        {
        //            Header = code?.Name,
        //            Content = new CodeDetailsUserControl() { DataContext = code }
        //        });
        //    }
        //}
        this.CodeNamesTreeView.BindItems(this.Codes);
}