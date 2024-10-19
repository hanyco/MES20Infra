using Web.UI.Components.Shared;
using Microsoft.AspNetCore.Components;
using Mes.Security;
using System.Net.Http;
using System.Net.Http.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using Library.DesignPatterns.Behavioral.Observation;
using Microsoft.Extensions.Caching.Memory;
using Library.Interfaces;

namespace Mes.Security;
public partial class AspNetUserDetailsComponent
{
    protected override async Task OnInitializedAsync()
    {
        await this.OnLoadAsync();
    }

    public async Task SaveData()
    {
        if (this.DataContext.Id == default)
        {
            var apiResult = await _http.PostAsJsonAsync($"aspnetuser/", DataContext);
        }
        else
        {
            var apiResult = await _http.PutAsJsonAsync($"aspnetuser/{this.DataContext.Id}/", DataContext);
        }

        MessageComponent.Show("Save Data", "Data saved successfully.");
    }

    public MessageComponent MessageComponent { get; set; }

    [Parameter]
    public long? EntityId { get; set; }

    [Parameter]
    public EventCallback<long?>? EntityIdChanged { get; set; }

    public void BackButton_OnClick()
    {
        this._navigationManager.NavigateTo("/System/AspNetUser");
    }
}