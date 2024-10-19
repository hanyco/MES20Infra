using Web.UI.Components.Shared;
using Mes.HumanResources.Dtos;
using System.Net.Http;
using System.Net.Http.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Mes.HumanResources;
public partial class PeopleListComponent
{
    protected override async Task OnLoadAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        if (!string.IsNullOrEmpty(token))
        {
            _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        try
        {
            var apiResult = await _http.GetFromJsonAsync<IEnumerable<PersonDto>>($"person/").ToListAsync();
            this.DataContext = apiResult;
        }
        catch (HttpRequestException ex)when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            _navigationManager.NavigateToLogin("/login");
        }
    }
}