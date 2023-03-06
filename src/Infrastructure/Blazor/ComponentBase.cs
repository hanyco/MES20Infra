using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace HanyCo.Infra.Blazor;

public abstract class ComponentBase<TDataContextType, TPageDataContextType> : ComponentBase
    where TDataContextType : new()
{
    public ComponentBase()
    {
        //this.StateHasChanged();
    }
    private TDataContextType? _dataContext;

    [Parameter]
    public TDataContextType? DataContext { get => this._dataContext; set => this._dataContext = value ?? new(); }

    [Parameter]
    public TPageDataContextType? PageDataContext { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder) => 
        base.BuildRenderTree(builder);

    protected override void OnAfterRender(bool firstRender) => 
        base.OnAfterRender(firstRender);

    protected override Task OnAfterRenderAsync(bool firstRender) => 
        base.OnAfterRenderAsync(firstRender);

    protected override bool ShouldRender() => base.ShouldRender();
}