using HanyCo.Infra.Security.Identity;
using Library.Types;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace HanyCo.Infra.Security.DataSources;

public partial class InfraSecDbContext : IdentityDbContext<
    InfraIdentityUser,
    InfraIdentityRole,
    Id,
    InfraIdentityUserClaim,
    InfraIdentityUserRole,
    InfraIdentityUserLogin,
    InfraIdentityRoleClaim,
    InfraIdentityUserToken>
{
}
