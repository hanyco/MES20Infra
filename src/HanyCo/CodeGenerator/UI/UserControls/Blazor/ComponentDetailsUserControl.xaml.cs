using HanyCo.Infra.CodeGen.Domain.ViewModels;
using HanyCo.Infra.UI.Dialogs;

namespace HanyCo.Infra.UI.Pages.Blazor;

/// <summary>
/// Interaction logic for ComponentDetailsUserControl.xaml
/// </summary>
public partial class ComponentDetailsUserControl
{
    public ComponentDetailsUserControl()
        => this.InitializeComponent();

    public void Initialize(IBlazorComponentCodeService service, IModuleService moduleService)
        => base.Initialize(service);

    protected override Task OnBindDataAsync(bool isFirstBinding)
        => base.OnBindDataAsync(isFirstBinding);

    private void EmptyPageDataContextPropertyNameRadioButton_Checked(object sender, System.Windows.RoutedEventArgs e)
        => this.ViewModel!.PageDataContextProperty = null;

    private void HasPageDataContextPropertyNameRadioButton_Checked(object sender, System.Windows.RoutedEventArgs e)
        => this.SetDataContextProperty();

    private void PageDataContextPropertyComboBox_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
    {
        if (this.ViewModel is null)
        {
            return;
        }

        this.PageDataContextPropertyComboBox.SelectedItem = this.ViewModel.PageDataContextProperty;
        if (this.PageDataContextPropertyComboBox.SelectedItem is null)
        {
            this.EmptyPageDataContextPropertyNameRadioButton.IsChecked = true;
        }
        else
        {
            this.HasPageDataContextPropertyNameRadioButton.IsChecked = true;
        }
    }

    private void PageDataContextPropertyComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        => this.SetDataContextProperty();

    private void SelectViewModelButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        if (this.ViewModel is null)
        {
            return;
        }

        var resp = SelectCqrsDialog.Show<DtoViewModel>(new("Select Component ViewModel", SelectCqrsDialog.LoadEntity.Dto, SelectCqrsDialog.FilterDto.ViewModel));
        if (resp is null or { IsSucceed: false } or { Value: null } || resp.Value is not DtoViewModel dto)
        {
            return;
        }
        this.ViewModel.PageDataContext = dto;
        this.SetDataContextProperty();
        this.RebindDataContext();
    }

    private void SetDataContextProperty()
    {
        if (this.ViewModel is null)
        {
            return;
        }

        if (this.ViewModel?.PageDataContextProperty is not null)
        {
            this.ViewModel.PageDataContextProperty.ValidateOnPropertySet = false;
        }

        this.ViewModel!.PageDataContextProperty = this.PageDataContextPropertyComboBox.SelectedItem.Cast().As<PropertyViewModel>();
        if (this.ViewModel?.PageDataContextProperty is not null)
        {
            this.ViewModel.PageDataContextProperty.ValidateOnPropertySet = true;
        }
    }
}