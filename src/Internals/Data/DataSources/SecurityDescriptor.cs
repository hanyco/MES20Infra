using System;
using System.Collections.Generic;

namespace HanyCo.Infra.Internals.Data.DataSources
{
    public partial class SecurityDescriptor
    {
        public SecurityDescriptor()
        {
            EntitySecurities = new HashSet<EntitySecurity>();
            SecurityClaims = new HashSet<SecurityClaim>();
        }

        public Guid Id { get; set; }
        public bool IncludeChildren { get; set; }
        public byte Strategy { get; set; }
        public string Name { get; set; } = null!;
        public bool? IsEnabled { get; set; }

        public virtual ICollection<EntitySecurity> EntitySecurities { get; set; }
        public virtual ICollection<SecurityClaim> SecurityClaims { get; set; }
    }
}
