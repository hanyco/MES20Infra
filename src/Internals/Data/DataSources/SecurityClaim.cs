using System;
using System.Collections.Generic;

namespace HanyCo.Infra.Internals.Data.DataSources
{
    public partial class SecurityClaim
    {
        public Guid Id { get; set; }
        public string ClaimType { get; set; } = null!;
        public string? ClaimValue { get; set; }
        public Guid SecurityDescriptorId { get; set; }

        public virtual SecurityDescriptor SecurityDescriptor { get; set; } = null!;
    }
}
