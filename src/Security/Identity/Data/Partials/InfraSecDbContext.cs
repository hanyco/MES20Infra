using HanyCo.Infra.Security.Identity;
using HanyCo.Infra.Security.Model;

using Library.Types;

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
