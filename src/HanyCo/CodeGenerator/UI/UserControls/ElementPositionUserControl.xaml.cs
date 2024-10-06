using HanyCo.Infra.CodeGen.Domain.ViewModels;

using System.ComponentModel;
using System.Windows.Controls;



namespace HanyCo.Infra.UI.UserControls;

/// <summary>
/// Interaction logic for ElementPositionUserControl.xaml
/// </summary>
public partial class ElementPositionUserControl : UserControl, INotifyPropertyChanged
{
    public ElementPositionUserControl()
    {
        this.InitializeComponent();
        this.DataContextChanged += this.ElementPositionUserControl_DataContextChanged;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void ElementPositionUserControl_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is UiBootstrapPositionViewModel oldValue)
        {
            oldValue.PropertyChanged -= this.Position_PropertyChanged;
        }

        if (e.NewValue is UiBootstrapPositionViewModel newValue)
        {
            newValue.PropertyChanged += this.Position_PropertyChanged;
        }
    }

    private void Position_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        => PropertyChanged?.Invoke(sender, e);
}