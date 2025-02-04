﻿


using HanyCo.Infra.CodeGen.Domain.Services;
using HanyCo.Infra.CodeGen.Domain.ViewModels;

using Library.Wpf.Windows.Controls;

namespace HanyCo.Infra.UI.UserControls.Blazor;

public class BlazorComponentUserControlBase : AsyncDataBindUserControl
{
    public BlazorComponentUserControlBase()
    {
        this.CodeService = DI.GetService< IBlazorComponentCodeService>();
        this.Service = DI.GetService<IBlazorComponentService>();
        this.IsEnabled = false;
        this.DataContextChanged += this.BlazorComponentUserControlBase_DataContextChanged;
    }

    public UiComponentViewModel? ViewModel
    {
        get => this.DataContext.Cast().As<UiComponentViewModel>();
        set => this.DataContext = value;
    }

    protected IBlazorComponentCodeService CodeService { get; private set; }
    protected IBlazorComponentService Service { get; private set; }

    public virtual void Initialize(IBlazorComponentCodeService service)
        => this.CodeService ??= service;

    protected override Task OnBindDataAsync(bool isFirstBinding)
        => Task.CompletedTask;

    private void BlazorComponentUserControlBase_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        => this.IsEnabled = this.DataContext is not null;
}