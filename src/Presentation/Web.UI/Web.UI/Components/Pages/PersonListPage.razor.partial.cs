using Web.UI.Components.Shared;
using HumanResources.Pages;
using System;
using System.Linq;
using System.Threading.Tasks;
using Library.DesignPatterns.Behavioral.Observation;
using Microsoft.Extensions.Caching.Memory;

namespace HumanResources.Pages;
public partial class PersonListPage
{
    protected override async Task OnInitializedAsync()
    {
        await this.OnLoadAsync();
    }

    public MessageComponent MessageComponent { get; set; }
}