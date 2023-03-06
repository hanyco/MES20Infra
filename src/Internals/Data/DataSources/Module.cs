using System;
using System.Collections.Generic;

namespace HanyCo.Infra.Internals.Data.DataSources
{
    public partial class Module
    {
        public Module()
        {
            CqrsSegregates = new HashSet<CqrsSegregate>();
            CrudCodes = new HashSet<CrudCode>();
            Dtos = new HashSet<Dto>();
            UiPages = new HashSet<UiPage>();
        }

        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public Guid Guid { get; set; }
        public long? ParentId { get; set; }

        public virtual ICollection<CqrsSegregate> CqrsSegregates { get; set; }
        public virtual ICollection<CrudCode> CrudCodes { get; set; }
        public virtual ICollection<Dto> Dtos { get; set; }
        public virtual ICollection<UiPage> UiPages { get; set; }
    }
}
