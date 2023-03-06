using HanyCo.Infra.UI.Helpers;
using Library.CodeGeneration.Models;

namespace HanyCo.Infra.UI.UserControls.Blazor;

/// <summary>
/// Interaction logic for CodeDetailsUserControl.xaml
/// </summary>
public partial class CodeDetailsUserControl
{
    public CodeDetailsUserControl() => this.InitializeComponent();

    public Code Code
    {
        get => this.DataContext?.To<Code>() ?? Code.Empty;
        set
        {
            if (this.DataContext?.To<Code>() == value)
            {
                return;
            }

            this.DataContext = value;
        }
    }

    private async void SaveButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        if (this.DataContext is Code code)
        {
            await SourceCodeHelper.SaveToFileAsync(code);
        }
    }
}