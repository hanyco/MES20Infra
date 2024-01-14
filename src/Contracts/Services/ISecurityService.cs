using HanyCo.Infra.Security.Model;

using Library.Results;

namespace HanyCo.Infra.Services;

public interface ISecurityService
{
    Task<Result> CreateUserAsync(InfraIdentityUser user, string password);

    Task<Result> DeleteRoleByIdAsync(Guid id);

    Task<Result> DeleteUserByIdAsync(Guid id);

    IEnumerable<InfraIdentityRole> GetRoles();

    Task<InfraIdentityUser> GetUserByIdAsync(Guid id);

    IEnumerable<InfraIdentityUser> GetUsers();

    Task<Result> LogInAsync(string username, string password, bool isPersist);

    Task<Result> LogOutAsync();

    Task<Result> SetUserClaimAsync(InfraIdentityUser user, string claimType, string claimValue);

    Task<Result> UpdateUserAsync(InfraIdentityUser user);
}