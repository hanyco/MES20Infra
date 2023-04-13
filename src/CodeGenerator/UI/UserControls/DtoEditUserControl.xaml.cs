using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Contracts.ViewModels;

using HanyCo.Infra.UI.Services;
using HanyCo.Infra.UI.ViewModels;

using Library.ComponentModel;
using Library.Exceptions.Validations;
using Library.Validations;
using Library.Wpf.Dialogs;

using Microsoft.WindowsAPICodePack.Dialogs;

namespace HanyCo.Infra.UI.UserControls;

/// <summary>
/// Interaction logic for DtoEditUserControl.xaml
/// </summary>
public partial class DtoEditUserControl : UserControl, IAsyncBindable, IUnidirectionalViewModel<DtoViewModel?>, IInitialzable, ISupportReadOnly
{
    public static readonly DependencyProperty? SelectedPropertyProperty = ControlHelper.GetDependencyProperty<PropertyViewModel?, DtoEditUserControl>(nameof(SelectedProperty));
    private int _maxPropId;
    private IModuleService _moduleService = null!;

    public DtoEditUserControl()
        => this.InitializeComponent();

    public bool IsReadOnly { get; set; }

    public PropertyViewModel? SelectedProperty
    {
        get => (PropertyViewModel?)this.GetValue(SelectedPropertyProperty);
        set => this.SetValue(SelectedPropertyProperty, value);
    }

    public DtoViewModel? ViewModel => this.DataContext.Cast().As<DtoViewModel>();

    public async Task BindAsync()
    {
        Check.NotNull(this._moduleService, () => new ValidationException($"Please call `{nameof(Initialize)}` method."));
        var modules = await this._moduleService.GetAllAsync();
        _ = this.ModulesComboBox.BindItemsSource(modules, nameof(ModuleViewModel.Name));
    }

    public void Initialize()
        => this._moduleService = DI.GetService<IModuleService>();

    public void RefreshState(DtoViewModel? viewModel)
    {
        this.DataContext = viewModel;
        if (viewModel is null)
        {
            this.PropertyDetails.ViewModel = null;
        }

        this.PropertyDetails.IsEnabled = viewModel is not null;
    }

    private void DeletePropertyButton_Click(object sender, RoutedEventArgs e)
        => this.DeleteSelectedProperties();

    private bool DeleteSelectedProperties()
    {
        if (this.IsReadOnly)
        {
            return false;
        }

        if (this.ViewModel is null)
        {
            return false;
        }

        var props = this.PropertiesListView.SelectedItems.Cast<PropertyViewModel>()?.ToList();
        if (props?.Any() is not true)
        {
            return false;
        }
        if (MsgBox2.AskWithWarn("Are you sure you want to delete selected properties?") != TaskDialogResult.Yes)
        {
            return false;
        }

        foreach (var prop in props)
        {
            var deletedPropId = prop.Id;
            _ = this.ViewModel.Properties.Remove(prop);
            if (deletedPropId is not null and > 0)
            {
                this.ViewModel.DeletedProperties.Add(prop);
            }
        }
        return true;
    }

    private void NewPropertyButton_Click(object sender, RoutedEventArgs e)
    {
        if (this.IsReadOnly)
        {
            return;
        }

        this.ViewModel.NotNull(nameof(this.ViewModel)).Properties.Add(new()
        {
            Name = $"NewProperty",
            Id = --this._maxPropId
        });
    }

    private void PropertiesListView_KeyDown(object sender, KeyEventArgs e)
    {
        if (this.IsReadOnly)
        {
            return;
        }

        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
        {
            bool? handle = e.Key switch
            {
                Key.Delete => this.DeleteSelectedProperties(),
                _ => null
            };
            e.Handled = handle ?? false;
        }
    }

    private void PropertiesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems is null or { Count: 0 or < 0 })
        {
            return;
        }
        this.SelectedProperty = e.AddedItems[0].Cast().As<PropertyViewModel>();
        if (this.SelectedProperty is null)
        {
            return;
        }

        this.PropertyDetails.RebindDataContext(this.SelectedProperty);
        this.PropertyDetails.IsEnabled = this.PropertyDetails.ViewModel is not null;
    }
}