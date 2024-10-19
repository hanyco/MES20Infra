using Web.UI.Components.Shared;
using Mes.HumanResources.Dtos;
using System.Net.Http;
using System.Net.Http.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Web.UI.Components.Pages;

namespace Mes.HumanResources;
public partial class AspNetUsersListComponent
{
    [Inject] private ILocalStorageService _localStorage { get; set; }

    protected override async Task OnLoadAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");

        if (!string.IsNullOrEmpty(token))
        {
            _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        try
        {
            var apiResult = await _http.GetFromJsonAsync<IEnumerable<AspNetUserDto>>("aspnetuser/").ToListAsync();
            this.DataContext = apiResult;

            StateHasChanged();
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            _navigationManager.NavigateToLogin("/login");
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            
        }
    }
}