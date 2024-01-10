using Library.Data.Markers;

using Microsoft.AspNetCore.Identity;

namespace HanyCo.Infra.Security.Model;

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