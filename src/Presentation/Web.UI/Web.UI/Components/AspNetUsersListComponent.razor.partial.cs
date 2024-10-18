using System;
using Web.UI.Components.Shared;
using Mes.Security;
using System.Net.Http;
using System.Net.Http.Json;
using System.Linq;
using System.Threading.Tasks;
using Library.DesignPatterns.Behavioral.Observation;
using Microsoft.Extensions.Caching.Memory;
using Library.Interfaces;

namespace Mes.Security;
public partial class AspNetUsersListComponent
{
    protected void NewButton_OnClick()
    {
        this._navigationManager.NavigateTo("/System/AspNetUser/details");
    }

    protected void EditButton_OnClick(String id)
    {
        this._navigationManager.NavigateTo($"/System/AspNetUser/details/{id.ToString()}");
    }

    protected async void DeleteButton_OnClick(String id)
    {
        var apiResult = await _http.DeleteFromJsonAsync($"aspnetuser/{id}/", typeof(bool));
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