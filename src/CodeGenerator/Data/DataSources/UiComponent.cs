using System;
using System.Collections.Generic;

namespace HanyCo.Infra.Internals.Data.DataSources
{
    public partial class UiComponent
    {
        public UiComponent()
        {
            UiComponentActions = new HashSet<UiComponentAction>();
            UiComponentProperties = new HashSet<UiComponentProperty>();
            UiPageComponents = new HashSet<UiPageComponent>();
        }

        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public Guid Guid { get; set; }
        public bool? IsEnabled { get; set; }
        public string? Caption { get; set; }
        public string ClassName { get; set; } = null!;
        public string NameSpace { get; set; } = null!;
        public long? PageDataContextId { get; set; }
        public long? PageDataContextPropertyId { get; set; }

        public virtual Dto? PageDataContext { get; set; }
        public virtual Property? PageDataContextProperty { get; set; }
        public virtual ICollection<UiComponentAction> UiComponentActions { get; set; }
        public virtual ICollection<UiComponentProperty> UiComponentProperties { get; set; }
        public virtual ICollection<UiPageComponent> UiPageComponents { get; set; }
    }
}
