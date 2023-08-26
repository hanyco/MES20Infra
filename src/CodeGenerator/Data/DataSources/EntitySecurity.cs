using System;
using System.Collections.Generic;

namespace HanyCo.Infra.Internals.Data.DataSources
{
    public partial class EntitySecurity
    {
        public Guid Id { get; set; }
        public Guid EntityId { get; set; }
        public Guid SecurityDescriptorId { get; set; }
        public bool? IsEnabled { get; set; }

        public virtual SecurityDescriptor SecurityDescriptor { get; set; } = null!;
    }
}
