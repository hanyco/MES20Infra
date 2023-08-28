using System;
using System.Collections.Generic;

namespace HanyCo.Infra.Internals.Data.DataSources
{
    public partial class Dto
    {
        public Dto()
        {
            CqrsSegregateParamDtos = new HashSet<CqrsSegregate>();
            CqrsSegregateResultDtos = new HashSet<CqrsSegregate>();
            Properties = new HashSet<Property>();
            UiComponents = new HashSet<UiComponent>();
            UiPages = new HashSet<UiPage>();
        }

        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string? NameSpace { get; set; }
        public long? ModuleId { get; set; }
        public string? DbObjectId { get; set; }
        public Guid Guid { get; set; }
        public string? Comment { get; set; }
        public bool IsParamsDto { get; set; }
        public bool IsResultDto { get; set; }
        public bool IsViewModel { get; set; }
        public bool? IsList { get; set; }

        public virtual Module? Module { get; set; }
        public virtual ICollection<CqrsSegregate> CqrsSegregateParamDtos { get; set; }
        public virtual ICollection<CqrsSegregate> CqrsSegregateResultDtos { get; set; }
        public virtual ICollection<Property> Properties { get; set; }
        public virtual ICollection<UiComponent> UiComponents { get; set; }
        public virtual ICollection<UiPage> UiPages { get; set; }
    }
}
