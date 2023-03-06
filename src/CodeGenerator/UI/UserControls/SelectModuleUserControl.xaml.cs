using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

using HanyCo.Infra.UI.Services;
using HanyCo.Infra.UI.ViewModels;

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

    private IModuleService _Service;

    #endregion Fields

    public event EventHandler<InitialItemEventArgs<IModuleService>> Initializing;

    public event EventHandler<ItemActedEventArgs<ModuleViewModel?>> SelectedModuleChanged;

    public SelectModuleUserControl()
        => this.InitializeComponent();

    public IEnumerable<ModuleViewModel> Modules { get => (IEnumerable<ModuleViewModel>)this.GetValue(ModulesProperty); set => this.SetValue(ModulesProperty, value); }

    public ModuleViewModel? SelectedModule { get => (ModuleViewModel)this.GetValue(SelectedModuleProperty); set => this.SetValue(SelectedModuleProperty, value); }

    public async Task InitializeAsync()
    {
        if (ControlHelper.IsDesignTime() || this.Modules is not null)
        {
            return;
        }

        _ = Check.MustBeNotNull(Initializing, () => "Please handle 'Initializing' event.").ThrowOnFail(this);
        var e = new InitialItemEventArgs<IModuleService>();
        Initializing(this, e);
        var service = e.Item;
        await this.InitializeAsync(service);
    }

    public async Task InitializeAsync([DisallowNull] IModuleService service)
    {
        _ = Check.MustBeArgumentNotNull(service).ThrowOnFail(this);
        this._Service = service;
        this.Modules = await this._Service.GetAllAsync();
        _ = this.ModulesComboBox.BindItemsSource(this.Modules, "Name", this.SelectedModule);
    }

    [Obsolete]
    private void ModulesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        => this.SelectedModule = e.GetSelection<ModuleViewModel>();

    private void OnSelectedModuleChanged()
        => SelectedModuleChanged?.Invoke(this, new ItemActedEventArgs<ModuleViewModel?>(this.SelectedModule));

    private async void SelectModuleUserControl_Loaded(object sender, RoutedEventArgs e)
        => await this.InitializeAsync();
}