using System.Windows;
using System.Windows.Controls;

using HanyCo.Infra.CodeGen.Domain.ViewModels;
using HanyCo.Infra.UI.Dialogs;
using HanyCo.Infra.UI.Helpers;

using Library.Exceptions;
using Library.Exceptions.Validations;
using Library.Validations;
using Library.Wpf.Dialogs;

using Microsoft.WindowsAPICodePack.Dialogs;

namespace HanyCo.Infra.UI.UserControls.Blazor;

/// <summary>
/// Interaction logic for ComponentActionsUserControl.xaml
/// </summary>
public partial class BlazorComponentActionsUserControl
{
    #region BlazorComponentActionViewModel SelectedAction

    public static readonly DependencyProperty SelectedActionProperty
        = ControlHelper.GetDependencyProperty<UiComponentButtonViewModelBase?, BlazorComponentActionsUserControl>(nameof(SelectedAction),
            onPropertyChanged: (me, e) =>
            {
                me.SelectedActionGrid.IsEnabled = me.SelectedAction is not null;
                me.DeleteActionButton.IsEnabled = me.SelectedAction is not null;
                if (me.SelectedAction is not null)
                {
                    me.SelectedAction.PropertyChanged += me.SelectedAction_PropertyChanged;
                }
            });

    public UiComponentButtonViewModelBase? SelectedAction
    {
        get => (UiComponentButtonViewModelBase)this.GetValue(SelectedActionProperty);
        set => this.SetValue(SelectedActionProperty, value);
    }

    private void SelectedAction_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(UiComponentButtonViewModelBase.Placement))
        {
            if (this.SelectedAction is not null)
            {
                var isButton = true;
                this.CaptionStackPanel.IsEnabled = this.ElementPositionUserControl.IsEnabled = isButton;
            }
        }
    }

    #endregion BlazorComponentActionViewModel SelectedAction

    public BlazorComponentActionsUserControl()
        => this.InitializeComponent();

    protected override Task OnBindDataAsync(bool _isFirstBinding)
    {
        if (this.ViewModel is null)
        {
            return Task.CompletedTask;
        }

        this.SelectedActionGrid.RebindDataContext();
        _ = this.ActionsListView.BindItemsSource(this.ViewModel.Actions);

        return Task.CompletedTask;
    }

    private void ActionsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedAction = this.ActionsListView.GetSelection<UiComponentButtonViewModelBase?>(e);
        this.SelectedAction = selectedAction;
        this.SelectedActionGrid.DataContext = this.SelectedAction;
        this.SelectedActionGrid.RebindDataContext();
    }

    private void AutoCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        if (this.Initializing)
        {
            return;
        }

        this.AutoStackPanel.Visibility = Visibility.Visible;
        this.ManualStackPanel.Visibility = Visibility.Collapsed;
    }

    private void ClearRefCqrsButton_Click(object sender, RoutedEventArgs e)
    {
        if (this.SelectedAction is null)
        {
            return;
        }
        if (this.SelectedAction is UiComponentCqrsButtonViewModel cqrsButton)
        {
            cqrsButton.CqrsSegregate = null;
        }
    }

    private void DeleteActionButton_Click(object sender, RoutedEventArgs e)
    {
        if (this.SelectedAction is null)
        {
            throw new ValidationException("No property selected.");
        }
        var index = this.ViewModel!.Actions.IndexOf(this.SelectedAction);
        if (index is -1)
        {
            throw new ValidationException("Property not found.");
        }
        if (MsgBox2.AskWithWarn($"You are about to delete property: {this.SelectedAction.Name}.", "Ae you sure?") != TaskDialogResult.Yes)
        {
            return;
        }
        this.ViewModel!.Actions.RemoveAt(index);
        this.SelectedActionGrid.DataContext = this.ViewModel!.Actions.Count > index
            ? this.ViewModel!.Actions[index]
            : this.ViewModel!.Actions.LastOrDefault();
        this.SelectedActionGrid.RebindDataContext();
    }

    private void ManualCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        if (this.Initializing)
        {
            return;
        }

        this.ManualStackPanel.Visibility = Visibility.Visible;
        this.AutoStackPanel.Visibility = Visibility.Collapsed;
    }

    private async void NewActionButton_Click(object sender, RoutedEventArgs e)
    {
        this.ViewModel!.Actions.Add(this.Service.CreateUnboundAction());
        await this.BindDataAsync();
    }

    private async void SetRefCqrsButton_Click(object sender, RoutedEventArgs e)
    {
        Check.MustBe<ObjectNotFoundException>(this.SelectedAction is not null);
        if (!SelectCqrsDialog.Show(out CqrsViewModelBase? selectedItem, new("Select a segregation", EnumHelper.AddFlag(SelectCqrsDialog.LoadEntity.Queries, SelectCqrsDialog.LoadEntity.Commands))))
        {
            return;
        }
        if (this.SelectedAction is UiComponentCqrsButtonViewModel cqrsButton)
        {
            cqrsButton.CqrsSegregate = await selectedItem.FillAsync();
        }
    }
}