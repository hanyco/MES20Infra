using Microsoft.AspNetCore.Identity;

namespace HanyCo.Infra.Security.Model;

public sealed class InfraIdentityRole : IdentityRole<Guid>
{
    public InfraIdentityRole()
    {
    }

    public InfraIdentityRole(string roleName) : base(roleName)
    {
    }
}