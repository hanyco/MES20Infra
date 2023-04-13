using System.Collections.ObjectModel;
using System.Windows;
using HanyCo.Infra.UI.Services;
using HanyCo.Infra.UI.ViewModels;
using Library.ComponentModel;
using Library.Exceptions.Validations;
using Library.Validations;
using Library.Wpf.Dialogs;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace HanyCo.Infra.UI.Pages;

/// <summary>
/// Interaction logic for SecurityDescriptorEditorPage.xaml
/// </summary>
public partial class SecurityDescriptorEditorPage
{
    private readonly ISecurityDescriptorService _service;

    public SecurityDescriptorEditorPage(ILogger logger, ISecurityDescriptorService service)
        : base(logger)
    {
        this._service = service;
        this.InitializeComponent();
    }

    public SecurityDescriptorEditorPageViewModel ViewModel
    {
        get
        {
            if (this.DataContext is not SecurityDescriptorEditorPageViewModel)
            {
                this.DataContext = new SecurityDescriptorEditorPageViewModel();
            }

            return this.DataContext.Cast().As<SecurityDescriptorEditorPageViewModel>()!;
        }
    }

    protected override async Task OnBindDataAsync() =>
        await this.RunCodeBlock(async () =>
        {
            var all = await this._service.GetAllAsync();
            _ = this.ViewModel.AllSecurityDescriptors.ClearAndAddRange(all);
            this.ViewModel.HighlightedSecurityDescriptor = this.ViewModel.SecurityDescriptor = null;
        }, this.Logger, "Initialing…", "Ready");

    private void AddClaimButton_Click(object sender, RoutedEventArgs e)
    {
        Check.NotNull(this.ViewModel.SecurityDescriptor);

        var newItem = ClaimViewModel.NewDefault();
        this.ViewModel.SecurityDescriptor.ClaimSet.Add(newItem);
    }

    private void DeleteClaimButton_Click(object sender, RoutedEventArgs e)
    {
        Check.NotNull(this.ViewModel.SecurityDescriptor);

        var item = this.ViewModel.HighlightedClaim;
        if (item is null)
        {
            NoItemValidationException.Throw("No claim is selected.");
        }
        if (MsgBox2.AskWithWarn("Are you sure?") != Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult.Yes)
        {
            return;
        }
        _ = this.ViewModel.SecurityDescriptor.ClaimSet.Remove(item);
    }

    private async void DeleteSecDescButton_Click(object sender, RoutedEventArgs e)
    {
        this.Logger.Info("Ready");
        var selectedItem = this.GetSelectedItem();
        if (MsgBox2.AskWithWarn("Are you sure?") != TaskDialogResult.Yes)
        {
            return;
        }

        this.Logger.Debug("Deleting…");
        _ = await this._service.DeleteAsync(selectedItem);
        await this.RebindDataAsync();
        this.Logger.Info("Item deleted");
    }

    private async void EditSecDescButton_Click(object sender, RoutedEventArgs e)
    {
        this.ViewModel.SecurityDescriptor = await this._service.GetByIdAsync(this.GetSelectedItem().Id);
        this.Logger.Info("Ready");
    }

    private SecurityDescriptorViewModel GetSelectedItem() =>
        this.ViewModel.HighlightedSecurityDescriptor ?? throw new NoItemValidationException("No item is selected.");

    private void NewSecDescButton_Click(object sender, RoutedEventArgs e)
    {
        this.ViewModel.SecurityDescriptor = SecurityDescriptorViewModel.New();
        this.Logger.Info("Ready");
    }

    private void ResetButton_Click(object sender, RoutedEventArgs e)
    {
        this.ViewModel.SecurityDescriptor = null;
        this.Logger.Info("Ready");
    }

    private async void SaveToDbButton_Click(object sender, RoutedEventArgs e) =>
            await this.RunCodeBlock(async () =>
        {
            _ = await this._service.SaveViewModelAsync(this.ViewModel.SecurityDescriptor);
            await this.RebindDataAsync();
        }, this.Logger, "Saving…", "Item saved.");

    private void SecDescsListView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) =>
        this.ViewModel.HighlightedSecurityDescriptor = e.NewValue.Cast().As<SecurityDescriptorViewModel>();
}

public class SecurityDescriptorEditorPageViewModel : NotifyPropertyChanged
{
    private ClaimViewModel? _highlightedClaim;
    private SecurityDescriptorViewModel? _highlightedSecurityDescriptor;
    private SecurityDescriptorViewModel? _selectedSecurityDescriptor;
    public ObservableCollection<SecurityDescriptorViewModel> AllSecurityDescriptors { get; } = new();

    public ClaimViewModel? HighlightedClaim
    {
        get => this._highlightedClaim;

        set => this.SetProperty(ref this._highlightedClaim, value);
    }

    public SecurityDescriptorViewModel? HighlightedSecurityDescriptor
    {
        get => this._highlightedSecurityDescriptor;

        set => this.SetProperty(ref this._highlightedSecurityDescriptor, value);
    }

    public SecurityDescriptorViewModel? SecurityDescriptor
    {
        get => this._selectedSecurityDescriptor;

        set => this.SetProperty(ref this._selectedSecurityDescriptor, value);
    }
}