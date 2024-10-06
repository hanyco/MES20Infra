using System.Windows;
using System.Windows.Forms;

using HanyCo.Infra.CodeGen.Domain.ViewModels;

using Library.Data.SqlServer;
using Library.Wpf.Windows.UI;

using UI;

namespace HanyCo.Infra.UI.Pages;

/// <summary>
/// Interaction logic for SettingsPage.xaml
/// </summary>
public partial class SettingsPage
{
    public SettingsPage(ILogger logger) : base(logger) =>
        this.InitializeComponent();

    public SettingsModel ViewModel =>
        this.DataContext.Cast().To<SettingsModel>();

    private void Page_Loaded(object sender, RoutedEventArgs e) =>
        this.Reload();

    private void Reload() =>
        this.DataContext = SettingsService.Get();

    private void ReloadButton_Click(object sender, RoutedEventArgs e) =>
                this.Reload();

    private void Save() =>
        this.ViewModel.Save();

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        _ = this.ShowToastCheckBox.Focus();
        this.Save();
        this.Logger.Info("Settings saved");
    }

    private void SelectProjectRootButton_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new FolderBrowserDialog
        {
            InitialDirectory = this.ViewModel.projectSourceRoot
        };
        var dlgResult = dlg.ShowDialog(App.Current.MainWindow.GetWin32Window());
        if (dlgResult != DialogResult.OK)
        {
            return;
        }
        this.ViewModel.projectSourceRoot = dlg.SelectedPath;
    }

    private async void TestButton_Click(object sender, RoutedEventArgs e) =>
        await Sql.TryConnectAsync(this.ViewModel.connectionString).ShowOrThrowAsync("Test Connection", "ConnectionString is checked.", "ConnectionString is Ok.");

    private void ToastHelpButton_Click(object sender, RoutedEventArgs e) {}
}