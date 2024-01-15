using Microsoft.AspNetCore.Components;

namespace BlazorApp.Authentication;

public partial class UserListPage : ComponentBase
{
    private List<UserInfo> DataContext { get; }

    protected override Task OnInitializedAsync()
    {
        var users = this._service.GetUsers().Select(user => new UserInfo
        {
            Id = user.Id.ToString(),
            Email = user.Email,
            UserName = user.UserName
        });
        this.DataContext.AddRange(users);
        return Task.CompletedTask;
    }

    private async void DeleteUser(string userId) => await this._service.DeleteUserByIdAsync(Guid.Parse(userId));

    private void EditUser(string userId) =>
        this._Navigator.NavigateTo($"/authentication/user/{userId}");

    private void NewUser() =>
        this._Navigator.NavigateTo("/authentication/user");

    private class UserInfo
    {
        public string Email { get; set; }
        public string Id { get; set; }
        public string UserName { get; set; }
    }
}