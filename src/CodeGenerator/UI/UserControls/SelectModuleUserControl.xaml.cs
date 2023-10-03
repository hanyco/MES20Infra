using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

using Contracts.Services;
using Contracts.ViewModels;

using Library.EventsArgs;
using Library.Validations;

namespace HanyCo.Infra.UI.UserControls;

/// <summary>
/// Interaction logic for SelectModuleUserControl.xaml
/// </summary>
public partial class SelectModuleUserControl : UserControl
{
    #region Fields

    public static readonly DependencyProperty ModulesProperty
        = DependencyProperty.Register(nameof(Modules), typeof(IEnumerable<ModuleViewModel>), typeof(SelectModuleUserControl), new PropertyMetadata(null));

    public static readonly DependencyProperty SelectedModuleProperty
        = ControlHelper.GetDependencyProperty<ModuleViewModel, SelectModuleUserControl>(nameof(SelectedModule), onPropertyChanged: (e1, e2) => { e1.OnSelectedModuleChanged(); });

    #endregion Fields

    public event EventHandler<InitialItemEventArgs<IModuleService>> Initializing;

    public event EventHandler<ItemActedEventArgs<ModuleViewModel?>> SelectedModuleChanged;

    public SelectModuleUserControl()
        => this.InitializeComponent();

    public IEnumerable<ModuleViewModel> Modules { get => (IEnumerable<ModuleViewModel>)this.GetValue(ModulesProperty); set => this.SetValue(ModulesProperty, value); }

    public ModuleViewModel? SelectedModule { get => (ModuleViewModel)this.GetValue(SelectedModuleProperty); set => this.SetValue(SelectedModuleProperty, value); }

    public async Task InitializeAsync()
    {
        if (ControlHelper.IsDesignTime())
        {
            return;
        }

        if (this.Modules == null)
        {
            IModuleService moduleService;
            if (Initializing != null)
            {
                var e = new InitialItemEventArgs<IModuleService>();
                this.Initializing(this, e);
                moduleService = e.Item.NotNull();
            }
            else
            {
                moduleService = DI.GetService<IModuleService>();
            }
            this.Modules = await moduleService.GetAllAsync();
            this.ModulesComboBox.BindItemsSource(this.Modules, nameof(ModuleViewModel.Name), this.SelectedModule);
        }
    } 

    [Obsolete]
    private void ModulesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        => this.SelectedModule = e.GetSelection<ModuleViewModel>();

    private void OnSelectedModuleChanged()
        => SelectedModuleChanged?.Invoke(this, new ItemActedEventArgs<ModuleViewModel?>(this.SelectedModule));

    private async void SelectModuleUserControl_Loaded(object sender, RoutedEventArgs e)
        => await this.InitializeAsync();
}