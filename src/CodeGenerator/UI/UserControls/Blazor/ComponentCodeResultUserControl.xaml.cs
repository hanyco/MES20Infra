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

    private void ComponentCodeResultUserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        //this.DataContext.As<>()
    }

    private void OnCodesChanged()
    {
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
    }
}