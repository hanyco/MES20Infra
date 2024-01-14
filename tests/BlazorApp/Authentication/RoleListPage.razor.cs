using Microsoft.AspNetCore.Components;

namespace BlazorApp.Authentication;

public partial class RoleListPage : ComponentBase
{
    private List<RoleInfo> DataContext { get; }

    protected override Task OnInitializedAsync()
    {
        var roles = this._service.GetRoles().Select(role => new RoleInfo
        {
            Id = role.Id.ToString(),
            Name = role.Name
        });
        this.DataContext.AddRange(roles);
        return Task.CompletedTask;
    }

    private async void DeleteUser(string userId) => 
        await this._service.DeleteRoleByIdAsync(Guid.Parse(userId));

    private void EditRole(string roleId) =>
        this._navigator.NavigateTo($"/authentication/role/{roleId}");

    private void NewRole() =>
        this._navigator.NavigateTo("/authentication/ole");

    private class RoleInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}