using System.ComponentModel.DataAnnotations;

using HanyCo.Infra.Security.Identity;

using Library.Exceptions;
using Library.Mapping;
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
            var user = await this._UserManager
                            .Users
                            .Where(user => user.Id == Guid.Parse(this.Id)).FirstOrDefaultAsync();
            Check.MustBeNotNull(user, () => new ObjectNotFoundException("User"));
            return UserDetailPageViewModel.FromInfraIdentityUser(user);
        }
        UserDetailPageViewModel createUser() =>
            new(null);
    }

    private async void SaveData()
    {
        _ = await updateUserAsync();

        async Task<Result> updateUserAsync()
        {
            var updateResult = await this._UserManager.UpdateAsync(this.DataContext.ToInfraIdentityUser());
            var result = updateResult.Succeeded
                ? Result.Success
                : Result.CreateFailure("Error on updating the user", errors: updateResult.Errors.Select(x => ((object)x.Code, (object)x.Description)));
            return result;
        }
    }
}

public sealed class UserDetailPageViewModel(in InfraIdentityUser infraIdentityUser)
{
    public int AccessFailedCount { get; } = infraIdentityUser?.AccessFailedCount ?? 0;
    public string ConcurrencyStamp { get; } = infraIdentityUser?.ConcurrencyStamp;

    [Required]
    public string Email { get; set; }

    public string Id { get; } = infraIdentityUser?.Id.ToString();
    public bool LockoutEnabled { get; set; }
    public DateTimeOffset? LockoutEnd { get; } = infraIdentityUser?.LockoutEnd;

    [Required]
    public string Password { get; set; }

    [Required]
    [Compare(nameof(Password))]
    public string PasswordConfirm { get; set; }

    public string PhoneNumber { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
    public string SecurityStamp { get; } = infraIdentityUser?.SecurityStamp;
    public bool TwoFactorEnabled { get; } = infraIdentityUser?.TwoFactorEnabled ?? false;

    [Required]
    public string UserName { get; set; }

    public static UserDetailPageViewModel FromInfraIdentityUser(InfraIdentityUser user)
    {
        var mapper = new Mapper();
        var result = mapper.Map<UserDetailPageViewModel>(user, new(user));
        return result;
    }

    public InfraIdentityUser ToInfraIdentityUser()
    {
        var mapper = new Mapper();
        var result = mapper.Map<InfraIdentityUser>(this);
        return result;
    }

    public override string ToString()
        => this.UserName?.ToString();
}