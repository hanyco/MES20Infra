using Web.UI.Components.Shared;
using Microsoft.AspNetCore.Components;
using Mes.HumanResourcesManagement;
using System.Net.Http.Headers;
using System.Net;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Blazored.LocalStorage;
using Mes.HumanResourcesManagement.Dtos;
using System.Net.Http;
using System.Net.Http.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using Library.DesignPatterns.Behavioral.Observation;
using Microsoft.Extensions.Caching.Memory;
using Library.Interfaces;

namespace Mes.HumanResourcesManagement;
public partial class PersonDetailsComponent
{
    protected override async Task OnInitializedAsync()
    {
        await this.OnLoadAsync();
    }

    public async Task SaveData()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        if (!string.IsNullOrEmpty(token))
        {
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        try
        {
            if (this.DataContext.Id == default)
            {
                var apiResult = await _http.PostAsJsonAsync($"person/", DataContext);
            }
            else
            {
                var apiResult = await _http.PutAsJsonAsync($"person/{this.DataContext.Id}/", DataContext);
                MessageComponent.Show("Save Data", "Data saved successfully.");
            }
        }
        catch (HttpRequestException ex)when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            _navigationManager.NavigateToLogin("/login");
        }
    }

    public MessageComponent MessageComponent { get; set; }

    [Parameter]
    public long? EntityId { get; set; }

    [Parameter]
    public EventCallback<long?>? EntityIdChanged { get; set; }

    public void BackButton_OnClick()
    {
        this._navigationManager.NavigateTo("/Mes/HumanResourcesManagement/Dtos/Person");
    }
}