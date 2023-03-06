using Microsoft.AspNetCore.Components;

namespace HanyCo.Infra.Blazor;

public abstract class PageBase<TDataContextType> : ComponentBase
{
    [Parameter]
    public TDataContextType? DataContext { get; set; }
}