using Web.UI.Components.Shared;
using System;
using HumanResources.Pages;
using Microsoft.AspNetCore.Components;
using System.Linq;
using System.Threading.Tasks;
using Library.DesignPatterns.Behavioral.Observation;
using Microsoft.Extensions.Caching.Memory;

namespace HumanResources.Pages;
public partial class PersonDetailsPage
{
    protected override async Task OnInitializedAsync()
    {
        await this.OnLoadAsync();
    }

    public MessageComponent MessageComponent { get; set; }

    [Parameter]
    public Int64? Id { get; set; }
}