using System.Net.Http.Headers;
using Web.UI.Components.Shared;
using Microsoft.AspNetCore.Components;
using Mes.System.Security;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Net.Http;
using System.Net.Http.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net;

namespace Mes.System.Security;
public partial class AspNetUserDetailsComponent
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
                var apiResult = await _http.GetFromJsonAsync<AspNetUserDto>($"aspnetuser/{entityId}/");
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
