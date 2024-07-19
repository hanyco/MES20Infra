using System.Windows.Controls;



using Library.Validations;
using Library.Wpf.Dialogs;

namespace HanyCo.Infra.UI.UserControls;

/// <summary>
/// Interaction logic for SecurityClaimCollectorUserControl.xaml
/// </summary>
public partial class SecurityClaimCollectorUserControl : UserControl
{
    private Func<IEnumerable<ClaimViewModel>>? _autoGenerateClaimEventHandler;
    private SecurityClaimExplorer? _securityClaimExplorer;

    public SecurityClaimCollectorUserControl() =>
        this.InitializeComponent();

    public IEnumerable<ClaimViewModel> ClaimViewModels
    {
        get => this.ClamsListBox.Items.Cast<ClaimViewModel>();
        set => this.ClamsListBox.Items.ClearAndAdd(value);
    }

    public void HandleAutoGenerateClaimEvent(Func<IEnumerable<ClaimViewModel>>? handler)
    {
        this._autoGenerateClaimEventHandler = handler;
        this.RefreshState();
    }

    private async void AddClaimButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        if (this._securityClaimExplorer == null)
        {
            this._securityClaimExplorer = new();
            await this._securityClaimExplorer.InitializeAsync();
        }
        _ = HostDialog.ShowDialog(this._securityClaimExplorer, "Select Security Claim", "Select a security claim."
            , _ => Check.If(this._securityClaimExplorer.SelectedClaim is null
            , () => "Please select a security claim."))
            .BreakOnFail();

        var selectedClaim = this._securityClaimExplorer.SelectedClaim;
        if (this.ClaimViewModels.Contains(selectedClaim))
        {
            return;
        }

        _ = this.ClamsListBox.Items.Add(selectedClaim);
    }

    private void GenerateClaimButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        var generatedClaims = this._autoGenerateClaimEventHandler?.Invoke();
        var newClaims = generatedClaims?.Compact().Except(this.ClaimViewModels);
        newClaims?.ForEach(x => this.ClamsListBox.Items.Add(x));
    }

    private void Me_Loaded(object sender, System.Windows.RoutedEventArgs e) =>
        this.RefreshState();

    private void RefreshState() =>
        this.GenerateClaimButton.Visibility = this._autoGenerateClaimEventHandler == null
            ? System.Windows.Visibility.Collapsed
            : System.Windows.Visibility.Visible;

    private void RemoveClaimButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        if (!this.ClamsListBox.SelectedItems.Any())
        {
            return;
        }

        this.ClamsListBox.SelectedItems.Cast<ClaimViewModel>().ToList().ForEach(this.ClamsListBox.Items.Remove);
    }
}