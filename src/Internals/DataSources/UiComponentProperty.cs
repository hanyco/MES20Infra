using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace HanyCo.Infra.Internals.Data.DataSources
{
    [Table("UiComponentProperty", Schema = "infra")]
    public partial class UiComponentProperty
    {
        [Key]
        public long Id { get; set; }
        public long UiComponentId { get; set; }
        public long PropertyId { get; set; }
        public int ControlTypeId { get; set; }
        public Guid Guid { get; set; }
        public string Caption { get; set; }
        public long PositionId { get; set; }
        public bool? IsEnabled { get; set; }

        [ForeignKey(nameof(PositionId))]
        [InverseProperty(nameof(BootstrapPosition.UiComponentProperties))]
        public virtual BootstrapPosition Position { get; set; }
        [ForeignKey(nameof(PropertyId))]
        [InverseProperty("UiComponentProperties")]
        public virtual Property Property { get; set; }
        [ForeignKey(nameof(UiComponentId))]
        [InverseProperty("UiComponentProperties")]
        public virtual UiComponent UiComponent { get; set; }
    }
}
