using System.Net.Http.Headers;
using System.Net;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Web.UI.Components.Shared;
using Blazored.LocalStorage;
using Mes.HumanResourcesManagement.Dtos;
using System.Net.Http;
using System.Net.Http.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Mes.HumanResourcesManagement;
public partial class PersonDetailsComponent
{
    protected override async Task OnLoadAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        if (!string.IsNullOrEmpty(token))
        {
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
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
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            _navigationManager.NavigateToLogin("/login");
        }
    }
}
