using System;
using System.Collections.Generic;

namespace HanyCo.Infra.Internals.Data.DataSources
{
    public partial class SecurityClaim
    {
        public SecurityClaim()
        {
            EntityClaims = new HashSet<EntityClaim>();
            UserClaimAccesses = new HashSet<UserClaimAccess>();
        }

        public Guid Id { get; set; }
        public string Key { get; set; } = null!;
        public Guid? Parent { get; set; }

        public virtual ICollection<EntityClaim> EntityClaims { get; set; }
        public virtual ICollection<UserClaimAccess> UserClaimAccesses { get; set; }
    }
}
