using HanyCo.Infra.Security.Model;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace HanyCo.Infra.Security.DataSources;

public partial class InfraSecDbContext : IdentityDbContext<
    InfraIdentityUser,
    InfraIdentityRole,
    Guid,
    InfraIdentityUserClaim,
    InfraIdentityUserRole,
    InfraIdentityUserLogin,
    InfraIdentityRoleClaim,
    InfraIdentityUserToken>
{
}