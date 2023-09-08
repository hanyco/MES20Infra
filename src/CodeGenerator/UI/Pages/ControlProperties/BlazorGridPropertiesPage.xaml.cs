using System.Windows;

using HanyCo.Infra.CodeGeneration.FormGenerator.Blazor.Components;

namespace HanyCo.Infra.UI.Pages.ControlProperties;

/// <summary>
/// Interaction logic for BlazorGridPropertiesPage.xaml
/// </summary>
public partial class BlazorGridPropertiesPage : ControlPropertyPage
{
    private static readonly DependencyProperty BlazorGridProperty = ControlHelper.GetDependencyProperty<BlazorTable, BlazorGridPropertiesPage>(
        nameof(BlazorGrid),
        onPropertyChanged: (s, _) =>
        {
            s.DataContext = s.BlazorGrid;
            s.OnBlazorGridPropertyChanged(s.BlazorGrid);
        });

    public BlazorGridPropertiesPage()
        => this.InitializeComponent();

    public BlazorTable BlazorGrid
    {
        get => (BlazorTable)this.GetValue(BlazorGridProperty);
        set => this.SetValue(BlazorGridProperty, value);
    }

    protected override void OnOk() => base.OnOk();

    private void AddColumnButton_Click(object sender, RoutedEventArgs e) => this.BlazorGrid.Columns.Add(new("", ""));

    private void DeleteColumnButton_Click(object sender, RoutedEventArgs e)
    {
    }

    private void OnBlazorGridPropertyChanged(BlazorTable blazorGrid)
    {
        //this.BlazorGrid.DataTemplate.DataContextName
    }
}