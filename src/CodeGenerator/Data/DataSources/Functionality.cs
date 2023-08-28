using System;
using System.Collections.Generic;

namespace HanyCo.Infra.Internals.Data.DataSources
{
    public partial class Functionality
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public long? ModuleId { get; set; }
        public Guid Guid { get; set; }
        public string? Comment { get; set; }
        public long GetAllQueryId { get; set; }
        public long GetByIdQueryId { get; set; }
        public long InsertCommandId { get; set; }
        public long UpdateCommandId { get; set; }
        public long DeleteCommandId { get; set; }

        public virtual CqrsSegregate DeleteCommand { get; set; } = null!;
        public virtual CqrsSegregate GetAllQuery { get; set; } = null!;
        public virtual CqrsSegregate GetByIdQuery { get; set; } = null!;
        public virtual CqrsSegregate InsertCommand { get; set; } = null!;
        public virtual CqrsSegregate UpdateCommand { get; set; } = null!;
    }
}
