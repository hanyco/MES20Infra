using System.ComponentModel.DataAnnotations;

using HanyCo.Infra.Security.Identity;
using HanyCo.Infra.Security.Model;

using Library.Mapping;

namespace BlazorApp.Data;

public sealed class UserDetailPageViewModel(in InfraIdentityUser infraIdentityUser)
{
    public int AccessFailedCount { get; } = infraIdentityUser?.AccessFailedCount ?? 0;
    public string ConcurrencyStamp { get; } = infraIdentityUser?.ConcurrencyStamp;

    [Required]
    public string Email { get; set; }

    public string FirstName { get; set; }
    public string Id { get; } = infraIdentityUser?.Id.ToString();
    public InfraIdentityUser IdentityUser { get; } = infraIdentityUser;
    public string LastName { get; set; }
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

    public static async Task<UserDetailPageViewModel> FromInfraIdentityUserAsync(InfraIdentityUser user)
    {
        var mapper = new Mapper();
        var manager = DI.GetService<InfraUserManager>();
        var claims = await manager.GetClaimsAsync(user);
        var result = mapper.Map<UserDetailPageViewModel>(user, new(user))
            .With(x => x.FirstName = claims.ValueByKey(nameof(FirstName)))
            .With(x => x.LastName = claims.ValueByKey(nameof(LastName)));
        return result;
    }

    public (InfraIdentityUser User, bool IsNew) ToInfraIdentityUser()
    {
        var mapper = new Mapper();
        var isNew = this.IdentityUser == null;
        var buffer = isNew ? new() : mapper.Map<InfraIdentityUser>(this.IdentityUser);
        var result = mapper.Map(this, buffer);
        return (result, isNew);
    }

    public override string ToString()
        => this.UserName?.ToString();
}