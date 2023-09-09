using System;
using System.Collections.Generic;

namespace HanyCo.Infra.Internals.Data.DataSources
{
    public partial class UserClaimAccess
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ClaimId { get; set; }
        public int AccessType { get; set; }

        public virtual SecurityClaim Claim { get; set; } = null!;
    }
}
