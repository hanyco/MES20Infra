using System.Windows;
using HanyCo.Infra.UI.Services.Imp;
using HanyCo.Infra.UI.ViewModels;

using Library.Wpf.Dialogs;
using Library.Wpf.Windows.UI;

namespace HanyCo.Infra.UI.Pages;

/// <summary>
/// Interaction logic for SettingsPage.xaml
/// </summary>
public partial class SettingsPage
{
    public SettingsPage(ILogger logger)
        : base(logger)
        => this.InitializeComponent();

    public SettingsModel ViewModel
        => this.DataContext.CastAs<SettingsModel>()!;

    private void OpenConnectionStringBoxButton_Click(object sender, RoutedEventArgs e)
    {
        var settings = this.DataContext.CastAs<SettingsModel>()!;
        var (isOk, connectionString) = ConnectionStringDialog.ShowDlg(settings.connectionString);
        if (isOk is true && connectionString is not null)
        {
            settings.connectionString = connectionString;
        }
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
        => this.DataContext = SettingsService.Get();

    private Task SaveAsync()
        => SettingsService.SaveAsync(this.ViewModel);

    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        await this.SaveAsync();
        this.Logger.Info("Settings saved");
    }
}