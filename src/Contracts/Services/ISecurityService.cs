using HanyCo.Infra.Security.Model;

using Library.Results;

namespace HanyCo.Infra.Services;

public interface ISecurityService
{
    Task<Result> CreateAsync(InfraIdentityUser user, string password);

    Task<InfraIdentityUser> GetUserByIdAsync(Guid id);

    IEnumerable<InfraIdentityUser> GetUsers();

    Task<Result> LogInAsync(string username, string password, bool isPersist);

    Task<Result> LogOutAsync();

    Task<Result> SetClaimAsync(InfraIdentityUser user, string claimType, string claimValue);

    Task<Result> UpdateAsync(InfraIdentityUser user);
}