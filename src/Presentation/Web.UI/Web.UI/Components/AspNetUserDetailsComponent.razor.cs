using Web.UI.Components.Shared;
using Microsoft.AspNetCore.Components;
using Mes.Security;
using System.Net.Http;
using System.Net.Http.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Mes.Security;
public partial class AspNetUserDetailsComponent
{
    protected override async Task OnLoadAsync()
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
}