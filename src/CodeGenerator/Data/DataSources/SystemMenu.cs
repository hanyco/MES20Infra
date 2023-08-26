using System;
using System.Collections.Generic;

namespace HanyCo.Infra.Internals.Data.DataSources
{
    public partial class SystemMenu
    {
        public long Id { get; set; }
        public long? ParentId { get; set; }
        public Guid Guid { get; set; }
        public string Caption { get; set; } = null!;
        public string? Uri { get; set; }
    }
}
