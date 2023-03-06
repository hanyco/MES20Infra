using System.Collections.ObjectModel;
using System.Windows;
using HanyCo.Infra.UI.Services;
using HanyCo.Infra.UI.ViewModels;
using Library.Validations;
using Library.Wpf.Bases;

namespace HanyCo.Infra.UI.Pages;

/// <summary>
/// Interaction logic for SecurityDescriptorLookupPage.xaml
/// </summary>
public partial class SecurityDescriptorLookupPage
{
    public static readonly DependencyProperty ViewModelProperty =
        ControlHelper.GetDependencyProperty<ObservableCollection<SecurityDescriptorLookupPageViewModelItem>, SecurityDescriptorLookupPage>(nameof(ViewModel), defaultValue: new());

    private readonly IEnumerable<SecurityDescriptorViewModel> _selectedItems;

    private readonly ISecurityDescriptorService _service;

    public SecurityDescriptorLookupPage(ILogger logger, IEnumerable<SecurityDescriptorViewModel> selectedItems)
                    : base(logger)
    {
        (this._service, this._selectedItems) = (DI.GetService<ISecurityDescriptorService>(), selectedItems);
        this.InitializeComponent();
    }

    public IEnumerable<SecurityDescriptorViewModel> SelectedItems => this.ViewModel.Where(x => x.IsChecked).Select(x => x.SecurityDescriptor);

    public ObservableCollection<SecurityDescriptorLookupPageViewModelItem> ViewModel
    {
        get => (ObservableCollection<SecurityDescriptorLookupPageViewModelItem>)this.GetValue(ViewModelProperty);
        set => this.SetValue(ViewModelProperty, value);
    }

    protected override async Task OnBindDataAsync()
    {
        var initiallySelectedIds = this._selectedItems.Select(x => x.Id).ToArray();
        var secs = await this._service.GetAllAsync();
        Check.If(secs.Any(), () => new Library.Exceptions.ObjectNotFoundException("No Security Description is defined."));
        this.ViewModel = secs.Select(x => new SecurityDescriptorLookupPageViewModelItem(initiallySelectedIds.Contains(x.Id), x)).ToObservableCollection();
    }

    public class SecurityDescriptorLookupPageViewModelItem : ViewModelBase
    {
        public SecurityDescriptorLookupPageViewModelItem(bool isChecked, SecurityDescriptorViewModel securityDescriptor)
            => (this.IsChecked, this.SecurityDescriptor) = (isChecked, securityDescriptor);

        public bool IsChecked { get; }
        public SecurityDescriptorViewModel SecurityDescriptor { get; }

        public override string ToString()
            => this.SecurityDescriptor.ToString();
    }
}