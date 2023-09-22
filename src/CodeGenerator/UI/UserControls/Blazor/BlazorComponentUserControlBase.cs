using Contracts.Services;
using Contracts.ViewModels;

using Library.Wpf.Windows.Controls;

namespace HanyCo.Infra.UI.UserControls.Blazor;

public class BlazorComponentUserControlBase : AsyncDataBindUserControl
{
    public BlazorComponentUserControlBase()
    {
        this.CodeService = DI.GetService< IBlazorComponentCodingService>();
        this.Service = DI.GetService<IBlazorComponentService>();
        this.IsEnabled = false;
        this.DataContextChanged += this.BlazorComponentUserControlBase_DataContextChanged;
    }

    public UiComponentViewModel? ViewModel
    {
        get => this.DataContext.Cast().As<UiComponentViewModel>();
        set => this.DataContext = value;
    }

    protected IBlazorComponentCodingService CodeService { get; private set; }
    protected IBlazorComponentService Service { get; private set; }

    public virtual void Initialize(IBlazorComponentCodingService service)
        => this.CodeService ??= service;

    protected override Task OnBindDataAsync(bool isFirstBinding)
        => Task.CompletedTask;

    private void BlazorComponentUserControlBase_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        => this.IsEnabled = this.DataContext is not null;
}