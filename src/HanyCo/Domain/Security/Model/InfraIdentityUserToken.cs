using Microsoft.AspNetCore.Identity;

namespace HanyCo.Infra.Security.Model;

public sealed class InfraIdentityUserToken : IdentityUserToken<Guid>
{
}