using BlazorApp.Data;

using Library.Exceptions;
using Library.Results;
using Library.Validations;

using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp.Authentication;

public partial class UserDetailPage : ComponentBase
{
    [Parameter]
    public string Id { get; set; }

    [Parameter]
    public EventCallback<long?>? IdChanged { get; set; }

    public Shared.MessageComponent MessageComponent { get; set; }

    private UserDetailPageViewModel DataContext { get; set; }

    protected override async Task OnInitializedAsync()
    {
        this.DataContext = this.Id.IsNullOrEmpty()
            ? createUser()
            : await initializeUser();

        async Task<UserDetailPageViewModel> initializeUser()
        {
            var user = await this._UserManager.Users.Where(user => user.Id == Guid.Parse(this.Id)).FirstOrDefaultAsync();
            Check.MustBeNotNull(user, () => new ObjectNotFoundException("User"));
            return await UserDetailPageViewModel.FromInfraIdentityUserAsync(user);
        }
        UserDetailPageViewModel createUser() =>
            new(null);
    }

    private async void SaveData()
    {
        var result = await CatchResultAsync(updateUserAsync).ThrowOnFailAsync();

        async Task<Result> updateUserAsync()
        {
            var (user, isNew) = this.DataContext.ToInfraIdentityUser();

            var saveResultBuffer = isNew
                ? await this._UserManager.CreateAsync(user, this.DataContext.Password)
                : await this._UserManager.UpdateAsync(user);
            if (!saveResultBuffer.Succeeded)
            {
                return Result.CreateFailure("Error on saving the user", errors: saveResultBuffer.Errors.ToResultErrors());
            }

            saveResultBuffer = await this._UserManager.SetClaimAsync(user, nameof(UserDetailPageViewModel.FirstName), this.DataContext.FirstName);
            if (!saveResultBuffer.Succeeded)
            {
                return Result.CreateFailure("Error on setting user's claims", errors: saveResultBuffer.Errors.ToResultErrors());
            }
            saveResultBuffer = await this._UserManager.SetClaimAsync(user, nameof(UserDetailPageViewModel.LastName), this.DataContext.LastName);
            if (!saveResultBuffer.Succeeded)
            {
                return Result.CreateFailure("Error on setting user's claims", errors: saveResultBuffer.Errors.ToResultErrors());
            };
            return Result.Success;
        }
    }
}