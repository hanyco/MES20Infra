using HanyCo.Infra.Security.Model;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HanyCo.Infra.Security.Identity.Entity;

public sealed class InfraUserManager(
    IUserStore<InfraIdentityUser> store,
    IOptions<IdentityOptions> optionsAccessor,
    IPasswordHasher<InfraIdentityUser> passwordHasher,
    IEnumerable<IUserValidator<InfraIdentityUser>> userValidators,
    IEnumerable<IPasswordValidator<InfraIdentityUser>> passwordValidators,
    ILookupNormalizer keyNormalizer,
    IdentityErrorDescriber errors,
    IServiceProvider services,
    ILogger<UserManager<InfraIdentityUser>> logger)
    : UserManager<InfraIdentityUser>(store,
        optionsAccessor,
        passwordHasher,
        userValidators,
        passwordValidators,
        keyNormalizer,
        errors,
        services,
        logger)
{
}