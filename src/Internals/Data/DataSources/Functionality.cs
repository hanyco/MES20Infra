using System;
using System.Collections.Generic;

namespace HanyCo.Infra.Internals.Data.DataSources
{
    public partial class Functionality
    {
        public Functionality()
        {
            CqrsSegregates = new HashSet<CqrsSegregate>();
            Dtos = new HashSet<Dto>();
        }

        public long Id { get; set; }
        public string? Name { get; set; }
        public long? ModuleId { get; set; }
        public Guid Guid { get; set; }
        public string? Comment { get; set; }

        public virtual ICollection<CqrsSegregate> CqrsSegregates { get; set; }
        public virtual ICollection<Dto> Dtos { get; set; }
    }
}
