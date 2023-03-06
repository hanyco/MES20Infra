using System;
using System.Collections.Generic;

namespace HanyCo.Infra.Internals.Data.DataSources
{
    public partial class CrudCode
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string? DbObjectId { get; set; }
        public long? ModuleId { get; set; }
        public long? GetCqrsSegregateId { get; set; }
        public long? GetByIdCqrsSegregateId { get; set; }
        public long? CreateCqrsSegregateId { get; set; }
        public long? UpdateCqrsSegregateId { get; set; }
        public long? DeleteCqrsSegregateId { get; set; }
        public string CqrsCodeNameSpace { get; set; } = null!;
        public string DtoCodeNameSpace { get; set; } = null!;
        public Guid Guid { get; set; }

        public virtual Module? Module { get; set; }
    }
}
