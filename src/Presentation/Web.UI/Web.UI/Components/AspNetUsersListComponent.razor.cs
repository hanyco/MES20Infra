using Web.UI.Components.Shared;
using Mes.System.Security;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net;

namespace Mes.System.Security;
public partial class AspNetUsersListComponent
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
            var apiResult = await _http.GetFromJsonAsync<IEnumerable<AspNetUserDto>>($"aspnetuser/").ToListAsync();
            this.DataContext = apiResult;
        }
        catch (HttpRequestException ex)when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            _navigationManager.NavigateToLogin("/login");
        }
    }
}