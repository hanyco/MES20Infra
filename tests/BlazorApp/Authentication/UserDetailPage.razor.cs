using BlazorApp.Data;
using BlazorApp.Shared;

using Library.Exceptions;
using Library.Results;
using Library.Validations;

using Microsoft.AspNetCore.Components;

namespace BlazorApp.Authentication;

public partial class UserDetailPage : ComponentBase
{
    [Parameter]
    public string Id { get; set; }

    [Parameter]
    public EventCallback<long?>? IdChanged { get; set; }

    public MessageComponent MessageComponent { get; set; }

    private UserDetailPageViewModel DataContext { get; set; }

    protected override async Task OnInitializedAsync()
    {
        this.DataContext = this.Id.IsNullOrEmpty()
            ? createUser()
            : await initializeUser();

        async Task<UserDetailPageViewModel> initializeUser()
        {
            var user = await this._securityService.GetUserByIdAsync(Guid.Parse(this.Id));
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

            Result saveResultBuffer;
            if (isNew)
            {
                saveResultBuffer = await this._securityService.CreateAsync(user, this.DataContext.Password);
            }
            else
            {
                saveResultBuffer = await this._securityService.UpdateAsync(user);
            }
            if (!saveResultBuffer.IsSucceed)
            {
                return saveResultBuffer;
            }
            saveResultBuffer = await this._securityService.SetClaimAsync(user, nameof(UserDetailPageViewModel.FirstName), this.DataContext.FirstName);
            if (!saveResultBuffer.IsSucceed)
            {
                return saveResultBuffer;
            }
            saveResultBuffer = await this._securityService.SetClaimAsync(user, nameof(UserDetailPageViewModel.LastName), this.DataContext.LastName);
            if (!saveResultBuffer.IsSucceed)
            {
                return saveResultBuffer;
            };
            return Result.Success;
        }
    }
}