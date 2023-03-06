using System;
using System.Collections.Generic;

namespace HanyCo.Infra.Internals.Data.DataSources
{
    public partial class UiBootstrapPosition
    {
        public UiBootstrapPosition()
        {
            UiComponentActions = new HashSet<UiComponentAction>();
            UiComponentProperties = new HashSet<UiComponentProperty>();
            UiPageComponents = new HashSet<UiPageComponent>();
        }

        public long Id { get; set; }
        public int? Order { get; set; }
        public int? Row { get; set; }
        public int? Col { get; set; }
        public int? ColSpan { get; set; }

        public virtual ICollection<UiComponentAction> UiComponentActions { get; set; }
        public virtual ICollection<UiComponentProperty> UiComponentProperties { get; set; }
        public virtual ICollection<UiPageComponent> UiPageComponents { get; set; }
    }
}
