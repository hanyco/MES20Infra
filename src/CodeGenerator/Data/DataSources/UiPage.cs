using System;
using System.Collections.Generic;

namespace HanyCo.Infra.Internals.Data.DataSources
{
    public partial class UiPage
    {
        public UiPage()
        {
            UiPageComponents = new HashSet<UiPageComponent>();
        }

        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string ClassName { get; set; } = null!;
        public Guid Guid { get; set; }
        public string NameSpace { get; set; } = null!;
        public long ModuleId { get; set; }
        public string? Route { get; set; }
        public long? DtoId { get; set; }

        public virtual Dto? Dto { get; set; }
        public virtual Module Module { get; set; } = null!;
        public virtual ICollection<UiPageComponent> UiPageComponents { get; set; }
    }
}
