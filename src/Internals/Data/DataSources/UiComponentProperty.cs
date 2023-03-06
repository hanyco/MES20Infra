﻿using System;
using System.Collections.Generic;

namespace HanyCo.Infra.Internals.Data.DataSources
{
    public partial class UiComponentProperty
    {
        public long Id { get; set; }
        public long UiComponentId { get; set; }
        public long? CqrsSegregateId { get; set; }
        public long? PropertyId { get; set; }
        public int ControlTypeId { get; set; }
        /// <summary>
        /// مکان در کامپوننت
        /// </summary>
        public long PositionId { get; set; }
        public string Caption { get; set; } = null!;
        public bool? IsEnabled { get; set; }
        public string? ControlExtraInfo { get; set; }

        public virtual UiBootstrapPosition Position { get; set; } = null!;
        public virtual Property? Property { get; set; }
        public virtual UiComponent UiComponent { get; set; } = null!;
    }
}
