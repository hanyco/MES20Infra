using Web.UI.Components.Shared;
using HumanResources.Dtos;
using System.Net.Http;
using System.Net.Http.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HumanResources;
public partial class PeopleListComponent
{
    protected override async Task OnLoadAsync()
    {
        var apiResult = await _http.GetFromJsonAsync<IEnumerable<PersonDto>>($"person/").ToListAsync();
        this.DataContext = apiResult;
    }
}