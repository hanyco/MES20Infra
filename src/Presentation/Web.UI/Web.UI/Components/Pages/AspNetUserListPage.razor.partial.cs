using Web.UI.Components.Shared;
using Mes.Infra.Auth.Pages;
using System;
using System.Linq;
using System.Threading.Tasks;
using Library.DesignPatterns.Behavioral.Observation;
using Microsoft.Extensions.Caching.Memory;

namespace Mes.Infra.Auth.Pages;
public partial class AspNetUserListPage
{
    protected override async Task OnInitializedAsync()
    {
        await this.OnLoadAsync();
    }

    public MessageComponent MessageComponent { get; set; }
}