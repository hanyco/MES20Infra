using System;
using System.Collections.Generic;

namespace HanyCo.Infra.Internals.Data.DataSources
{
    public partial class UiPageComponent
    {
        public long Id { get; set; }
        public Guid Guid { get; set; }
        public long PageId { get; set; }
        public long UiComponentId { get; set; }
        /// <summary>
        /// مکان در پیج
        /// </summary>
        public long? PositionId { get; set; }

        public virtual UiPage Page { get; set; } = null!;
        public virtual UiBootstrapPosition? Position { get; set; }
        public virtual UiComponent UiComponent { get; set; } = null!;
    }
}
