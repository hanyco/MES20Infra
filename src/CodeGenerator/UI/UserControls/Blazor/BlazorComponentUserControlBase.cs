using Contracts.Services;

using HanyCo.Infra.UI.ViewModels;

using Library.Wpf.Windows.Controls;

namespace HanyCo.Infra.UI.UserControls.Blazor;

public class BlazorComponentUserControlBase : AsyncDataBindUserControl
{
    public BlazorComponentUserControlBase()
    {
        this.Service = null!;
        this.IsEnabled = false;
        this.DataContextChanged += this.BlazorComponentUserControlBase_DataContextChanged;
    }

    public UiComponentViewModel? ViewModel
    {
        get => this.DataContext.Cast().As<UiComponentViewModel>();
        set => this.DataContext = value;
    }

    protected IBlazorCodingService Service { get; private set; }

    public virtual void Initialize(IBlazorCodingService service)
        => this.Service = service;

    protected override Task OnBindDataAsync(bool isFirstBinding)
        => Task.CompletedTask;

    private void BlazorComponentUserControlBase_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        => this.IsEnabled = this.DataContext is not null;
}