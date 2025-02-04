using System;
using Web.UI.Components.Shared;
using Mes.HumanResourcesManagement;
using System.Net.Http.Headers;
using System.Net;
using Blazored.LocalStorage;
using Mes.HumanResourcesManagement.Dtos;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Linq;
using System.Threading.Tasks;
using Library.DesignPatterns.Behavioral.Observation;
using Microsoft.Extensions.Caching.Memory;
using Library.Interfaces;

namespace Mes.HumanResourcesManagement;
public partial class PeopleListComponent
{
    protected void NewButton_OnClick()
    {
        this._navigationManager.NavigateTo("/Mes/HumanResourcesManagement/Dtos/Person/details");
    }

    protected void EditButton_OnClick(Int64 id)
    {
        this._navigationManager.NavigateTo($"/Mes/HumanResourcesManagement/Dtos/Person/details/{id.ToString()}");
    }

    protected async void DeleteButton_OnClick(Int64 id)
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        if (!string.IsNullOrEmpty(token))
        {
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        try
        {
            var apiResult = await _http.DeleteFromJsonAsync($"person/{id}/", typeof(bool));
            await OnInitializedAsync();
            MessageComponent.Show("Delete Entity", "Entity deleted.");
            this.StateHasChanged();
        }
        catch (HttpRequestException ex)when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            _navigationManager.NavigateToLogin("/login");
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await this.OnLoadAsync();
    }

    public MessageComponent MessageComponent { get; set; }
}