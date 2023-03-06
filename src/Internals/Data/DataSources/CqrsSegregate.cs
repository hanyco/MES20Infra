using System;
using System.Collections.Generic;

namespace HanyCo.Infra.Internals.Data.DataSources
{
    public partial class CqrsSegregate
    {
        public CqrsSegregate()
        {
            UiComponentActions = new HashSet<UiComponentAction>();
        }

        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string? CqrsNameSpace { get; set; }
        public int SegregateType { get; set; }
        public string? FriendlyName { get; set; }
        public string? Description { get; set; }
        public long ParamDtoId { get; set; }
        public long ResultDtoId { get; set; }
        public Guid Guid { get; set; }
        public string? Comment { get; set; }
        public long ModuleId { get; set; }
        public int CategoryId { get; set; }
        public long? FunctionalityId { get; set; }

        public virtual Functionality? Functionality { get; set; }
        public virtual Module Module { get; set; } = null!;
        public virtual Dto ParamDto { get; set; } = null!;
        public virtual Dto ResultDto { get; set; } = null!;
        public virtual ICollection<UiComponentAction> UiComponentActions { get; set; }
    }
}
