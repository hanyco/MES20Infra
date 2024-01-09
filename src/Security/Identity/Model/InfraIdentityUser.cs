using HanyCo.Infra.Security.Model;

using Library.Data.Markers;
using Library.Types;

namespace HanyCo.Infra.Security.Identity;

public sealed class InfraIdentityUser : IdentityUser<Guid>, IEntity
{
    public InfraIdentityUser()
    {
    }

    public InfraIdentityUser(string userName) : base(userName)
    {
    }

    //public long CompanyId { get; set; }
    //public DateTime? LastSignedIn { get; set; }
}
