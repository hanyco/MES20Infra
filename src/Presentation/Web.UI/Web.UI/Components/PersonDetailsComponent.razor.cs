using Web.UI.Components.Shared;
using Microsoft.AspNetCore.Components;
using Mes.HumanResources.Dtos;
using System.Net.Http;
using System.Net.Http.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Mes.HumanResources;
public partial class PersonDetailsComponent
{
    [Inject] ILocalStorageService _localStorage { get; }
    protected override async Task OnLoadAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        if (!string.IsNullOrEmpty(token))
        {
            _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        try
        {
            if (this.EntityId is { } entityId)
            {
                var apiResult = await _http.GetFromJsonAsync<PersonDto>($"person/{entityId}/");
                this.DataContext = apiResult;
            }
            else
            {
                this.DataContext = new();
            }
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            _navigationManager.NavigateToLogin("/login");
        }
    }
}
