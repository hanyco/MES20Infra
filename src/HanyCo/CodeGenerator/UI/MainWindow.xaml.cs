﻿using System.Windows;
using System.Windows.Controls;

using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.UI.Controls.Pages;
using HanyCo.Infra.UI.Pages;

using Library.EventsArgs;
using Library.Results;
using Library.Threading.MultistepProgress;
using Library.Windows;
using Library.Wpf.Dialogs;
using Library.Wpf.Windows.UI;

using Microsoft.Data.SqlClient;

namespace UI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public static readonly DependencyProperty IsInitiatedProperty =
        DependencyProperty.Register(nameof(IsInitiated), typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

    private readonly IEventualLogger _logger;
    private readonly InfraWriteDbContext _writeDbContext;

    public MainWindow(IEventualLogger logger, IProgressReport reportHost, InfraWriteDbContext writeDbContext)
    {
        this.InitializeComponent();
        this._writeDbContext = writeDbContext;
        this._logger = logger;
        reportHost.Reported += this.ReportHost_Reported;
        reportHost.Ended += this.ReportHost_Ended;
        this._logger.Logging += this.Logger_Logging;
        MsgBox2.DefaultWindow = this;
    }

    public static DependencyProperty CurrentPageTitleProperty { get; } = ControlHelper.GetDependencyProperty<string?, MainWindow>(propertyName: nameof(CurrentPageTitle), defaultValue: "(No page)");

    public string? CurrentPageTitle { get => (string)this.GetValue(CurrentPageTitleProperty); set => this.SetValue(CurrentPageTitleProperty, value); }

    private bool IsInitiated
    {
        get => (bool)this.GetValue(IsInitiatedProperty);
        set => this.SetValue(IsInitiatedProperty, value);
    }

    private void BlazorComponentMenuItem_Click(object sender, RoutedEventArgs e) =>
        this.Navigate<BlazorComponentGenertorPage>();

    private void BlazorPageMenuItem_Click(object sender, RoutedEventArgs e) =>
        this.Navigate<BlazorPageGeneratorPage>();

    private void CreateCommandMenuItem_Click(object sender, RoutedEventArgs e) =>
        this.Navigate<CqrsCommandDetailsPage>();

    private void CreateCrudMenuItem_Click(object sender, RoutedEventArgs e) =>
        this.Navigate<CreateTableCrudPage>();

    private void CreateDtoMenuItem_Click(object sender, RoutedEventArgs e) =>
        this.Navigate<DtoDetailsPage>();

    private void CreateQueryMenuItem_Click(object sender, RoutedEventArgs e) =>
        this.Navigate<CqrsQueryDetailsPage>();

    private void EditSecurityDescriptor_Click(object sender, RoutedEventArgs e)
    {
    }

    private void FunctionalityEditorManuItem_Click(object sender, RoutedEventArgs e) =>
        this.Navigate<FunctionalityEditorPage>();

    private void GenerateSourceWizard_Click(object sender, RoutedEventArgs e)
    {
        var wizardDlg = new SourceGeneratorWizardWindow();
        _ = wizardDlg.ShowDialog();
    }

    private void HelpAboutMenuItem_Click(object sender, RoutedEventArgs e)
    {
        var title = $"About {ApplicationHelper.Company} {ApplicationHelper.ApplicationTitle}";
        var productName = ApplicationHelper.ProductTitle;
        var version = $"Version {ApplicationHelper.Version}";
        var copr = $"{ApplicationHelper.Copyright}{Environment.NewLine}All rights reserved.";
        var description = $"{ApplicationHelper.Description}{Environment.NewLine}{Environment.NewLine}{version}{Environment.NewLine}{Environment.NewLine}{copr}";
        var warning = "Warning: This computer program is protected by copyright law and international treaties. Unauthorized reproduction or distribution of this program, or any portion of it, may result in severe civil and criminal penalties, and will be prosecuted to the maximum extent possible under the law.";
        MsgBox2.Inform(productName, description, title, detailsExpandedText: warning);
    }

    private void Log(object message)
        => this.StatusBarItem.RunInControlThread(() =>
            {
                var v = message switch
                {
                    LogRecord l => l.Message?.ToString(),
                    LogRecord<object> lo => lo.Message?.ToString()?.Trim(),
                    object o => o.ToString(),
                    _ => "Ready."
                };
                if (v.IsNullOrEmpty())
                {
                    return;
                }

                this.StatusBarItem.Content = v.Split(Environment.NewLine).Last().Trim();
                _ = this.StatusBarItem.Refresh();
                var log = LoggingHelper.Reformat(message);
                if (!log.IsNullOrEmpty())
                {
                    _ = this.LogListBox.Items.Add(log);
                }
            });

    private void Logger_Logging(object? sender, ItemActedEventArgs<LogRecord<object>> e)
        => this.Log(e.Item);

    private void Navigate<TPage>()
        where TPage : PageBase
    {
        var page = DI.GetService<TPage>()!;

        this.CurrentPageTitle = page.Title;
        this._logger.Debug($"Navigating to page: {page.Title}");
        this.PageTab.Header = page.Title;
        this.PageTab.IsSelected = true;
        _ = this.PageHostFrame.Navigate(page);
    }

    private void PageHostFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
    {
        if (e.Content.Cast().As<Page>() is { } page)
        {
            this.Title = $"{page.Title} - {ApplicationHelper.ApplicationTitle}";
        }
    }

    private void ReportHost_Ended(object? sender, ItemActedEventArgs<ProgressData?> e) =>
        this.RunInControlThread(() =>
        {
            this.StatusProgressBar.Visibility = Visibility.Collapsed;
            this.StatusProgressBar.Background = System.Windows.Media.Brushes.Blue;
            _ = this.StatusProgressBar.Refresh();
            _ = Taskbar.MainWindow.ProgressBar.SetState(TaskbarProgressState.None);
            this.Log(e.Item ?? "Ready.");
        });

    private void ReportHost_Reported(object? sender, ItemActedEventArgs<ProgressData> e) =>
        this.RunInControlThread(() =>
        {
            if (e.Item.Max != 0)
            {
                this.StatusProgressBar.Visibility = Visibility.Visible;
                this.StatusProgressBar.Maximum = e.Item.Max.Cast().ToInt(0);
                this.StatusProgressBar.Value = e.Item.Current.Cast().ToInt(0);
                this.StatusProgressBar.Background = System.Windows.Media.Brushes.Maroon;
                _ = Taskbar.MainWindow.ProgressBar.SetState(TaskbarProgressState.Normal).SetValue(this.StatusProgressBar.Value, this.StatusProgressBar.Maximum);
            }
            else
            {
                this.StatusProgressBar.Visibility = Visibility.Collapsed;
                this.StatusProgressBar.Background = System.Windows.Media.Brushes.Blue;
                _ = Taskbar.MainWindow.ProgressBar.SetState(TaskbarProgressState.None);
            }
            if (!e.Item.Description.IsNullOrEmpty())
            {
                this.Log(e.Item.Description);
            }
            _ = this.StatusProgressBar.Refresh();
            _ = this.StatusBarItem.Refresh();
        });

    private void SettingMenuItem_Click(object sender, RoutedEventArgs e) =>
        this.Navigate<SettingsPage>();

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        this.Title = ApplicationHelper.ApplicationTitle;
        try
        {
            this._logger.Debug("Connecting to database...");
            var created = await this._writeDbContext.Database.EnsureCreatedAsync();
            if (created)
            {
                MsgBox2.Warn(
                    "Database created.",
                    "Database not found. But created from schema information.",
                    footerText: "Please remember to initialize database, if required.",
                    footerIcon: Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardIcon.Warning);
            }

            this._logger.Debug("Ready.");
            this.IsInitiated = true;
        }
        catch(SqlException ex) when (ex.Message.Contains("Database 'MesInfra' already exists"))
        {
            this._logger.Debug("Database check: OK.");
            this.IsInitiated = true;
        }
        catch (SqlException ex) when (ex.ErrorCode == -2146232060)
        {
            var message = new NotificationMessage("Could not connect to database engine.", "SQL Server not found.", "SQL Server Connection Error", ex.Message, MessageLevel.Error, typeof(App));

            this._logger.Fatal(message.Text, this.Title);
            var closeButton = ButtonInfo.New("Close", (_, _) => this.Close());
            var continueButton = ButtonInfo.New("Continue", (o, e) => MsgBox2.GetOnButtonClick(o, e).Parent.Close(), useElevationIcon: true);
            _ = MsgBox2.Error(
                message.Instruction,
                message.Text,
                message.Title,
                detailsExpandedText: message.Details,
                window: this,
                controls: [closeButton, continueButton]);
        }
        catch (Exception ex)
        {
            _ = Result.Fail("Exception occurred on connecting to database", ex).ThrowOnFail();
        }
    }

    private void ClearLogsButton_Click(object sender, RoutedEventArgs e)
    {
        LogListBox.Items.Clear();
    }

    private void PageHostFrame_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
    {
        
    }

    private void PageHostFrame_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
    {
    }
}