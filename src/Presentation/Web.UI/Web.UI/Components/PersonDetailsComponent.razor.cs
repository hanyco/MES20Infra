using Web.UI.Components.Shared;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using HumanResources.Dtos;

namespace HumanResources;
public partial class PersonDetailsComponent
{
    protected override async Task OnLoadAsync()
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
}