using System.Windows.Controls;

using Contracts.Services;
using Contracts.ViewModels;

using Library.ComponentModel;

namespace HanyCo.Infra.UI.UserControls;

/// <summary>
/// Interaction logic for SecurityClaimExplorer.xaml
/// </summary>
public partial class SecurityClaimExplorer : UserControl, IAsyncInitialzable
{
    private readonly ISecurityService _securityService;

    public SecurityClaimExplorer()
    {
        this._securityService = DI.GetService<ISecurityService>();
        this.InitializeComponent();
    }

    public ClaimViewModel? SelectedClaim => this.ClaimTreeView.GetSelectedModel<ClaimViewModel>();

    public async Task InitializeAsync()
    {
        var claims = await this._securityService.GetAllAsync();
        var rootNode = new TreeViewItem { Header = "Security Claims" };
        EnumerableHelper.BuildTree(
            claims.Where(x => x.Parent == null),
            c => new TreeViewItem().With(x => x.DataContext = c),
            c => claims.Where(x => x.Parent?.Id == c.Id),
            t => rootNode.Items.Add(t),
            (p, c) => p.Items.Add(c)
            );
        _ = this.ClaimTreeView.Items.ClearAndAdd(rootNode);
    }
}