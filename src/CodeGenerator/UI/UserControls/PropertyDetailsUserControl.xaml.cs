﻿using Contracts.ViewModels;

using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.UI.Dialogs;
using HanyCo.Infra.UI.ViewModels;

namespace HanyCo.Infra.UI.UserControls;

public partial class PropertyDetailsUserControl
{
    public PropertyDetailsUserControl()
    {
        this.InitializeComponent();
        _ = this.PropertyTypesComboBox.BindItemsSource(EnumHelper.GetItems<PropertyType>());
    }

    public PropertyViewModel? ViewModel
    {
        get => this.DataContext.As<PropertyViewModel>();
        set => this.DataContext = value;
    }

    private void PropertyTypesComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) =>
        this.SelectDtoButton.IsEnabled = this.ViewModel?.Type is PropertyType.Dto;

    private void SelectDtoButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        if (this.ViewModel is null)
        {
            return;
        }
        if (SelectCqrsDialog.Show(out DtoViewModel? dto, new("Select DTO", SelectCqrsDialog.LoadEntity.Dto)))
        {
            this.ViewModel.Dto = dto;
        }
    }
}