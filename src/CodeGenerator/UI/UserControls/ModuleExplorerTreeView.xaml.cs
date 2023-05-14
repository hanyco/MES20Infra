using System.Windows;
using System.Windows.Controls;

using Contracts.Services;

namespace HanyCo.Infra.UI.UserControls;

/// <summary>
/// Interaction logic for ModuleExplorerTreeView.xaml
/// </summary>
public partial class ModuleExplorerTreeView : UserControl
{
    #region Fields

    private readonly IDtoService _DtoService;

    #endregion Fields

    public ModuleExplorerTreeView()
    {
        if (ControlHelper.IsDesignTime())
        {
            return;
        }

        this.BeginInitializing();
        this._DtoService = DI.GetService<IDtoService>()!;

        this.InitializeComponent();
    }

    public static DependencyProperty LoadDtosProperty { get; } = ControlHelper.GetDependencyProperty<bool, CqrsExplorerTreeView>(nameof(LoadDtos), onPropertyChanged: async (me, _) => await me.BindAsync());

    public bool IsInitializing { get; private set; }

    public bool LoadDtos { get => (bool)this.GetValue(LoadDtosProperty); set => this.SetValue(LoadDtosProperty, value); }

    public void BeginInitializing()
        => this.IsInitializing = true;

    public void EndInitializing()
        => this.IsInitializing = false;

    private async Task BindAsync()
    {
        if (this.IsInitializing)
        {
            return;
        }
    }
}