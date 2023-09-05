using System.Drawing;
using System.Windows;
using System.Windows.Controls;

using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.UI.Controls.Pages;
using HanyCo.Infra.UI.Pages;

using Library.EventsArgs;
using Library.Results;
using Library.Threading.MultistepProgress;
using Library.Windows;
using Library.Wpf.Dialogs;

using Microsoft.Data.SqlClient;

namespace UI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    #region Fields

    private readonly IEventualLogger _logger;
    private readonly InfraWriteDbContext _writeDbContext;

    #endregion Fields

    public MainWindow(InfraWriteDbContext writeDbContext, IEventualLogger logger, IProgressReport reportHost)
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

    private void EditSecurityDescriptor_Click(object sender, RoutedEventArgs e) =>
        this.Navigate<SecurityDescriptorEditorPage>();

    private void FunctionalityEditorManuItem_Click(object sender, RoutedEventArgs e) =>
        this.Navigate<FunctionalityEditorPage>();

    private void GenerateSourceWizard_Click(object sender, RoutedEventArgs e)
    {
        var wizardDlg = new SourceGeneratorWizardWindow();
        _ = wizardDlg.ShowDialog();
    }

    private void Log(object message)
    {
        this.StatusBarItem.Content = message switch
        {
            LogRecord l => l.Message,
            LogRecord<object> lo => lo.Message?.ToString()?.Trim(),
            object o => o.ToString(),
            _ => "Ready."
        };
        this.StatusBarItem.Refresh();
        var log = LoggingHelper.Reformat(message);
        if (!log.IsNullOrEmpty())
        {
            _ = this.LogListBox.Items.Add(log);
        }
    }

    private void Logger_Logging(object? sender, ItemActedEventArgs<object> e) =>
        this.Log(e.Item);

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
            this.Log(e.Item ?? "Ready.");
            this.StatusProgressBar.Refresh();
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
            }
            else
            {
                this.StatusProgressBar.Visibility = Visibility.Collapsed;
                this.StatusProgressBar.Background = System.Windows.Media.Brushes.Blue;
            }
            if (!e.Item.Description.IsNullOrEmpty())
            {
                this.Log(e.Item.Description);
            }
            this.StatusProgressBar.Refresh();
            this.StatusBarItem.Refresh();
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
                    footerText: "Please do not forget to initialize database, if required.",
                    footerIcon: Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardIcon.Warning);
            }

            this._logger.Debug("Ready.");
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
                controls: new Microsoft.WindowsAPICodePack.Dialogs.TaskDialogControl[] { closeButton, continueButton });
        }
        catch (Exception ex)
        {
            _ = Result.CreateFailure("Exception occurred on connecting to database", ex).ThrowOnFail();
        }
    }
}