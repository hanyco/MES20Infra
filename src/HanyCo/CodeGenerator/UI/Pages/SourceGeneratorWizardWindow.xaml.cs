using System.Globalization;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;

namespace HanyCo.Infra.UI.Pages;

/// <summary>
/// Interaction logic for SourceGeneratorWizardWindow.xaml
/// </summary>
public partial class SourceGeneratorWizardWindow : Window
{
    public SourceGeneratorWizardWindow()
        => this.InitializeComponent();

    private async Task CreateByDto()
        => await Task.CompletedTask;

    private async Task CreateNewSource()
        => await Task.CompletedTask;

    private async void SourceGeneratorWizardWindow_Loaded(object sender, RoutedEventArgs e)
    {
        var page = new TaskDialogPage()
        {
            Caption = "Source Generator",
            Heading = "Source Generator Wizard",
            AllowCancel = true,
            Text = "Select an option",
            Verification = new("Verification"),
            Expander = new TaskDialogExpander("Expander"),
            Buttons =
            {
                new TaskDialogCommandLinkButton("Generate &New Source", "Create new source from scratch using database table")
                {
                    Tag = 10
                },
                new TaskDialogCommandLinkButton("Generate by &DTO", "Create source using DTO which is created previousely.")
                {
                    Tag = 20
                },
                new TaskDialogCommandLinkButton("❗&Regenerate All Sources", $"Generate all sources using all DTOs.{Environment.NewLine}This may take several minutes.")
                {
                    Tag = 99,
                    Visible = false
                }
            },
            Footnote = new TaskDialogFootnote()
            {
                Text = "Note: By choosing any of above option, you'll get step forward.",
            }
        };

        var result = TaskDialog.ShowDialog(new WindowWrapper(new WindowInteropHelper(this).Handle), page);
        var wizrdResult = (result.Tag, result.Text?.ToLower(CultureInfo.InvariantCulture)) switch
        {
            (10, _) => this.CreateNewSource(),
            (20, _) => this.CreateByDto(),
            (_, "cancel") => CloseAsync(),
            _ => throw new NotImplementedException(),
        };
        await wizrdResult;

        async Task CloseAsync()
        {
            this.Close();
            await Task.CompletedTask;
        }
    }
}

public class WindowWrapper : System.Windows.Forms.IWin32Window
{
    public WindowWrapper(IntPtr handle) => this.Handle = handle;

    public WindowWrapper(Window window) => this.Handle = new WindowInteropHelper(window).Handle;

    public IntPtr Handle { get; }
}