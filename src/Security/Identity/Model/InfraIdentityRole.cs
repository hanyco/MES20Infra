using HanyCo.Infra.Security.Model;
using Library.Types;

namespace HanyCo.Infra.Security.Identity;

public sealed class InfraIdentityRole : IdentityRole<Id>, IRoleModel
{
    public InfraIdentityRole()
    {
    }

    public InfraIdentityRole(string roleName) : base(roleName)
    {
    }
}
