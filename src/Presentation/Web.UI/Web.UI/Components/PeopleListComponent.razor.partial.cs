using System;
using Web.UI.Components.Shared;
using HumanResources;
using System.Linq;
using System.Threading.Tasks;
using Library.DesignPatterns.Behavioral.Observation;
using Microsoft.Extensions.Caching.Memory;
using Library.Interfaces;

namespace HumanResources;
public partial class PeopleListComponent
{
    public  void NewButton_OnClick()
    {
        this._navigationManager.NavigateTo("/HumanResources/Person/details");
    }

    public  void EditButton_OnClick(Int64 id)
    {
        this._navigationManager.NavigateTo($"/HumanResources/Person/details/{id.ToString()}");
    }

    public  async void DeleteButton_OnClick(Int64 id)
    {
        var apiResult = await _http.DeleteFromJsonAsync("person/", id);
        await OnInitializedAsync();
        MessageComponent.Show("Delete Entity", "Entity deleted.");
        this.StateHasChanged();
    }

    protected override async Task OnInitializedAsync()
    {
        await this.OnLoadAsync();
    }

    public MessageComponent MessageComponent { get; set; }
}