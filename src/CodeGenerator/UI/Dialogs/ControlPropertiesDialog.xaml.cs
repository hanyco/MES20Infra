using HanyCo.Infra.UI.Pages.ControlProperties;

namespace HanyCo.Infra.UI.Dialogs;

/// <summary>
/// Interaction logic for ControlPropertiesDialog.xaml
/// </summary>
public partial class ControlPropertiesDialog
{
    private ControlPropertyPage _MyPage;

    public ControlPropertiesDialog()
        => this.InitializeComponent();

    public ControlPropertyPage MyPage
    {
        get => this._MyPage;
        set
        {
            this._MyPage = value;

            _ = this.MyFrame.Navigate(this.MyPage);
            this.Title = this.MyPage.Title;
        }
    }

    private void OkButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        this.DialogResult = true;
        this.Close();
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        if (this.DialogResult == true)
        {
            this.MyPage.Ok();
        }
    }
}