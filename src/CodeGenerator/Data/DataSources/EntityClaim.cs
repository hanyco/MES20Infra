using System;
using System.Collections.Generic;

namespace HanyCo.Infra.Internals.Data.DataSources
{
    public partial class EntityClaim
    {
        public Guid Id { get; set; }
        public Guid EntityId { get; set; }
        public Guid ClaimId { get; set; }

        public virtual SecurityClaim Claim { get; set; } = null!;
    }
}
