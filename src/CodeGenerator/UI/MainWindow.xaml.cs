using System.Windows;

using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.UI.Controls.Pages;
using HanyCo.Infra.UI.Pages;

using Library.EventsArgs;
using Library.Helpers;
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

    public MainWindow(InfraWriteDbContext writeDbContext, IEventualLogger logger, IMultistepProcess reportHost)
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

    private void BlazorComponentMenuItem_Click(object sender, RoutedEventArgs e)
        => this.Navigate<BlazorComponentGenertorPage>();

    private void BlazorPageMenuItem_Click(object sender, RoutedEventArgs e)
        => this.Navigate<BlazorPageGeneratorPage>();

    private void CreatCodeMenuIten_Click(object sender, RoutedEventArgs e)
        => this.Navigate<CodeDetailsPage>();

    private void CreateCommandMenuItem_Click(object sender, RoutedEventArgs e)
        => this.Navigate<CqrsCommandDetailsPage>();

    private void CreateCrudMenuItem_Click(object sender, RoutedEventArgs e)
        => this.Navigate<CreateTableCrudPage>();

    private void CreateDtoMenuItem_Click(object sender, RoutedEventArgs e)
        => this.Navigate<DtoDetailsPage>();

    private void CreateQueryMenuItem_Click(object sender, RoutedEventArgs e)
        => this.Navigate<CqrsQueryDetailsPage>();

    private void EditSecurityDescriptor_Click(object sender, RoutedEventArgs e)
        => this.Navigate<SecurityDescriptorEditorPage>();

    private void FunctionalityEditorManuItem_Click(object sender, RoutedEventArgs e)
        => this.Navigate<FunctionalityEditorPage>();

    private void GenerateSourceWizard_Click(object sender, RoutedEventArgs e)
    {
        var wizardDlg = new SourceGeneratorWizardWindow();
        _ = wizardDlg.ShowDialog();
    }

    private void log(object message)
    {
        this.StatusBarItem.Content = message switch
        {
            LogRecord l => l.Message,
            LogRecord<object> lo => lo.Message?.ToString()?.Trim(),
            object o => o.ToString(),
            _ => "Ready."
        };

        var log = LoggingHelper.Reformat(message);
        if (!log.IsNullOrEmpty())
        {
            _ = this.LogListBox.Items.Add(log);
        }
    }

    private void Logger_Logging(object? sender, Library.EventsArgs.ItemActedEventArgs<object> e)
        => this.log(e.Item);

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
        if (e.Content.As<PageBase>() is { } page)
        {
            this.Title = $"{page.Title} - {ApplicationHelper.ApplicationTitle}";
        }
    }

    private void ReportHost_Ended(object? sender, ItemActedEventArgs<ProgressData?> e)
        => this.RunInControlThread(() =>
        {
            this.StatusProgressBar.Visibility = Visibility.Collapsed;
            this.log(e.Item ?? "Ready.");
        });

    private void ReportHost_Reported(object? sender, ItemActedEventArgs<ProgressData> e)
        => this.RunInControlThread(() =>
        {
            if (e.Item.Max != 0)
            {
                this.StatusProgressBar.Visibility = Visibility.Visible;
                this.StatusProgressBar.Maximum = e.Item.Max.ToInt(0);
                this.StatusProgressBar.Value = e.Item.Current.ToInt(0);
            }
            if (!e.Item.Description.IsNullOrEmpty())
            {
                this.log(e.Item.Description);
            }
        });

    private void SettingMenuItem_Click(object sender, RoutedEventArgs e)
        => this.Navigate<SettingsPage>();

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        this.Title = ApplicationHelper.ApplicationTitle;
        try
        {
            this._logger.Debug("Connecting to database...");
            _ = await this._writeDbContext.Database.EnsureCreatedAsync();
            this._logger.Debug("Ready.");
        }
        catch (SqlException ex) when (ex.ErrorCode == -2146232060)
        {
            var message = new NotificationMessage("Could not connect to database engine.", "SQL Server not found.", "SQL Server Connection Error", ex.Message, MessageLevel.Error, typeof(App));

            this._logger.Fatal(message.Text, this.Title);
            var closeButton = ButtonInfo.New("Close", (_, _) => this.Close());
            var contButton = ButtonInfo.New("Continue", (o, e) => MsgBox2.GetOnButtonClick(o, e).Parent.Close(), useElevationIcon: true);
            _ = MsgBox2.Error(
                message.Instruction,
                message.Text,
                message.Title,
                detailsExpandedText: message.Details,
                window: this,
                controls: new Microsoft.WindowsAPICodePack.Dialogs.TaskDialogControl[] { closeButton, contButton });
        }
    }
}