using Web.UI.Components.Shared;
using System;
using Mes.HumanResources.Pages;
using Microsoft.AspNetCore.Components;
using System.Linq;
using System.Threading.Tasks;
using Library.DesignPatterns.Behavioral.Observation;
using Microsoft.Extensions.Caching.Memory;

namespace Mes.HumanResources.Pages;
public partial class AspNetUserDetailsPage
{
    protected override async Task OnInitializedAsync()
    {
        await this.OnLoadAsync();
    }

    public MessageComponent MessageComponent { get; set; }

    [Parameter]
    public Int64? Id { get; set; }
}