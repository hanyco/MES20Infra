﻿using System.Windows;
using System.Windows.Forms;

using Contracts.ViewModels;

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
        this.DataContext = SettingsService.Get();

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

    private void ToastHelpButton_Click(object sender, RoutedEventArgs e) =>
        Toast2.New().AddText("This is a sample toast notification.").Show();
}