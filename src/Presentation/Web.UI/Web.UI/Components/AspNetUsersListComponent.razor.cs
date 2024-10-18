using Web.UI.Components.Shared;
using Mes.Security;
using System.Net.Http;
using System.Net.Http.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Mes.Security;
public partial class AspNetUsersListComponent
{
    protected override async Task OnLoadAsync()
    {
        var apiResult = await _http.GetFromJsonAsync<IEnumerable<AspNetUserDto>>($"aspnetuser/").ToListAsync();
        this.DataContext = apiResult;
    }
}