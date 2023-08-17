using System.Collections.ObjectModel;
using System.Windows;
using HanyCo.Infra.UI.ViewModels;
using Library.Exceptions.Validations;
using Library.Validations;
using Library.Wpf.Dialogs;

namespace HanyCo.Infra.UI.UserControls;

/// <summary>
/// Interaction logic for SecurityItemUserControl.xaml
/// </summary>
public partial class EntitySecurityDescriptorViewUserControl
{
    private readonly ILogger _logger;

    public EntitySecurityDescriptorViewUserControl()
    {
        if (!ControlHelper.IsDesignTime())
        {
            this._logger = DI.GetService<ILogger>();
        }

        this.InitializeComponent();
    }

    public ObservableCollection<SecurityDescriptorViewModel> ViewModel
    {
        get
        {
            if (this.DataContext is not ObservableCollection<SecurityDescriptorViewModel>)
            {
                this.DataContext = new ObservableCollection<SecurityDescriptorViewModel>();
            }

            return this.DataContext.Cast().To<ObservableCollection<SecurityDescriptorViewModel>>();
        }
        set => this.DataContext = value;
    }

    private void AddSecurityDescriptorButton_Click(object sender, RoutedEventArgs e)
    {
        var viewModel = this.ViewModel.ToEnumerable();
        var lookupPage = new Pages.SecurityDescriptorLookupPage(this._logger, viewModel);
        if (HostDialog.Create(lookupPage).SetPrompt("Select a Security Description for selected property.").Show() != true)
        {
            return;
        }

        this.ViewModel.Clear();
        _ = viewModel.ForEach(this.ViewModel.Add);
    }

    private void DeleteItemsButton_Click(object sender, RoutedEventArgs e)
    {
        Check.MustBe(!this.SecurityDescriptorsListView.SelectedItems.Any(), () => new NoItemValidationException("No items selected."));

        var selectedItems = this.SecurityDescriptorsListView.SelectedItems.Cast<SecurityDescriptorViewModel>().ToList();
        foreach (var item in selectedItems)
        {
            _ = this.ViewModel.Remove(item);
        }
    }
}