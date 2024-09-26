using Web.UI.Components.Shared;
using Microsoft.AspNetCore.Components;
using HumanResources;
using System;
using System.Linq;
using System.Threading.Tasks;
using Library.DesignPatterns.Behavioral.Observation;
using Microsoft.Extensions.Caching.Memory;
using Library.Interfaces;

namespace HumanResources;
public partial class PersonDetailsComponent
{
    protected override async Task OnInitializedAsync()
    {
        await this.OnLoadAsync();
    }

    public MessageComponent MessageComponent { get; set; }

    [Parameter]
    public long? EntityId { get; set; }

    [Parameter]
    public EventCallback<long?>? EntityIdChanged { get; set; }

    public void BackButton_OnClick()
    {
        this._navigationManager.NavigateTo("/HumanResources/Person");
    }
}