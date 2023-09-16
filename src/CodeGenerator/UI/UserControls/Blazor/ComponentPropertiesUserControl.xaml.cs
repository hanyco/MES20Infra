using System.Windows;
using System.Windows.Controls;

using Contracts.Services;
using Contracts.ViewModels;

using HanyCo.Infra.CodeGeneration.FormGenerator.Blazor.Components;
using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.UI.Dialogs;
using HanyCo.Infra.UI.Pages.ControlProperties;

using Library.Exceptions.Validations;
using Library.Mapping;
using Library.Validations;
using Library.Wpf.Dialogs;

using Services.Helpers;

namespace HanyCo.Infra.UI.Pages.Blazor;

/// <summary>
/// Interaction logic for ComponentPropertiesUserControl.xaml
/// </summary>
public partial class ComponentPropertiesUserControl
{
    #region BlazorComponentPropertyViewModel SelectedProperty

    public static readonly DependencyProperty SelectedPropertyProperty
        = ControlHelper.GetDependencyProperty<UiComponentPropertyViewModel?, ComponentPropertiesUserControl>(nameof(SelectedProperty),
            onPropertyChanged: (me, e) =>
            {
                me.SelectedPropertyGrid.IsEnabled = me.SelectedProperty is not null;
                me.DeletePropertyButton.IsEnabled = me.SelectedProperty is not null;
                //await me.BindDataAsync();
            });

    public UiComponentPropertyViewModel? SelectedProperty
    {
        get => (UiComponentPropertyViewModel)this.GetValue(SelectedPropertyProperty);
        set => this.SetValue(SelectedPropertyProperty, value);
    }

    #endregion BlazorComponentPropertyViewModel SelectedProperty

    private IEntityViewModelConverter _converter;
    private DtoViewModel? _selectedDto;

    public ComponentPropertiesUserControl() => this.InitializeComponent();

    public ControlType? SelectedControlType { get; set; }
    public IEnumerable<UiComponentPropertyViewModel?>? SelectedProperties { get; set; }

    protected override async Task OnBindDataAsync(bool isFirstBinding)
    {
        if (isFirstBinding)
        {
            _ = this.ControlTypeComboBox.BindItemsSource(EnumHelper.GetItems<ControlType>());
        }
        if (this.ViewModel is null)
        {
            _ = this.SelectedPropertyComboBox.BindItemsSource(null);
            return;
        }
        if (this.SelectedPropertyComboBox.ItemsSource is null)
        {
            _ = this.SelectedPropertyComboBox.BindItemsSource(this.ViewModel.UiProperties.Select(x => x.Property));
        }
        this.BindPropertiesListView();

        await base.OnBindDataAsync(isFirstBinding);
    }

    private async void BindPropertiesListView()
    {
        if (this.ViewModel is null)
        {
            return;
        }
        this.ViewModel.UiProperties.Clear();
        var propertyService = DI.GetService<IPropertyService>();
        var codingService = DI.GetService<IBlazorComponentCodingService>();
        var properties = this.ViewModel.PageDataContextProperty is null
            ? await propertyService.GetByParentIdAsync(this.ViewModel.PageDataContext.Id!.Value)
            : await propertyService.GetByDtoIdAsync(this.ViewModel.PageDataContextProperty.Id!.Value);
        _ = this.ViewModel.UiProperties.AddRange(properties.Select(x => this._converter.ToUiComponentProperty(x)));
        _ = this.PropertiesListView.BindItemsSource(this.ViewModel.UiProperties);
    }

    private async void BrowserForDtoButton_Click(object sender, RoutedEventArgs e)
    {
        var resp = SelectCqrsDialog.Show<DtoViewModel>(new("Select DTO", SelectCqrsDialog.LoadEntity.Dto));
        if (resp is null or { IsSucceed: false } or { Value: null })
        {
            return;
        }
        var dto = resp.Value;
        this._selectedDto = dto;
        var controlType = ControlTypeHelper.ByDtoViewModel(dto);
        _ = this.SelectedProperty.NotNull(nameof(this.SelectedProperty))
                                 .ForMember(x => x.ControlType = controlType)
                                 .ForMember(x => x.Property ??= new())
                                 .Property
                                 .ForMember(x => x.Dto = dto)
                                 .ForMember(x => x.Name = dto.Name)
                                 .ForMember(x => x.IsList = controlType == ControlType.DataGrid)
                                 .ForMember(x => x.Comment = dto.Comment ?? string.Empty)
                                 .ForMember(x => x.Type = PropertyType.Dto)
                                 .ForMember(x => x.TypeFullName = dto.FullName);
        await this.BindDataAsync();
    }

    private void ClearRefPropertyButton_Click(object sender, RoutedEventArgs e)
        => this.SelectedPropertyComboBox.SelectedIndex = -1;

    private void ControlPropertiesButton_Click(object sender, RoutedEventArgs e)
    {
        switch (this.SelectedControlType)
        {
            case ControlType.DataGrid when this._selectedDto is null:
                _ = ThrowOnError(() => new RequiredValidationException("Please select a DTO."));
                break;

            case ControlType.DataGrid:
                setDataGrid();
                break;
        }

        void setDataGrid()
        {
            var grid = new BlazorTable();
            var dto = this._selectedDto;
            _ = grid.Columns.AddRange(dto.Properties.Select(x => new BlazorTableColumn(x.Name, x.DbObject.Name)).ToArray());

            var component = grid ?? new();
            ControlPropertiesDialog dlg = new() { MyPage = new BlazorGridPropertiesPage { BlazorGrid = component } };
            grid = dlg.ShowDialog() is true ? component : grid;
        }
    }

    [Obsolete]
    private void ControlTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        this.SelectedControlType = e.GetSelection<ControlType?>();
        this.ControlPropertiesButton.IsEnabled = this.Service.HasPropertiesPage(this.SelectedControlType);
    }

    private void DeletePropertyButton_Click(object sender, RoutedEventArgs e)
    {
        if (this.SelectedProperties?.Any() is not true)
        {
            throw new ValidationException("No property selected.");
        }
        if (MsgBox2.AskWithWarn("Are you sure?", "You are about to deleted the selected item(s).") != Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult.Yes)
        {
            return;
        }

        foreach (var property in this.SelectedProperties.Compact().ToList())
        {
            var index = this.ViewModel!.UiProperties.IndexOf(property);
            if (index is -1)
            {
                throw new ValidationException("Property not found.");
            }
            this.ViewModel!.UiProperties.RemoveAt(index);
            //this.SelectedPropertyGrid.DataContext = this.ViewModel!.UiProperties.Count > index
            //    ? this.ViewModel!.UiProperties[index]
            //    : this.ViewModel!.UiProperties.LastOrDefault();
        }
        //this.SelectedPropertyGrid.RebindDataContext();
    }

    private async void NewPropertyButton_Click(object sender, RoutedEventArgs e)
    {
        this.ViewModel!.UiProperties.Add(this.Service.CreateUnboundProperty());
        await this.BindDataAsync();
    }

    private void PropertiesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var prop = this.PropertiesListView.GetSelection<UiComponentPropertyViewModel>(e);
        this.SelectedProperty = prop;
        this.SelectedPropertyGrid.RebindDataContext(this.SelectedProperty);
        //await this.BindDataAsync();
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e) => this._converter = DI.GetService<IEntityViewModelConverter>();
}