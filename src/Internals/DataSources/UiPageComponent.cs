using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace HanyCo.Infra.Internals.Data.DataSources
{
    [Table("UiPageComponent", Schema = "infra")]
    public partial class UiPageComponent
    {
        [Key]
        public long Id { get; set; }
        public Guid Guid { get; set; }
        public long PageId { get; set; }
        public long UiComponentId { get; set; }
        public long PositionId { get; set; }

        [ForeignKey(nameof(PageId))]
        [InverseProperty(nameof(UiPage.UiPageComponents))]
        public virtual UiPage Page { get; set; }
        [ForeignKey(nameof(PositionId))]
        [InverseProperty(nameof(BootstrapPosition.UiPageComponents))]
        public virtual BootstrapPosition Position { get; set; }
        [ForeignKey(nameof(UiComponentId))]
        [InverseProperty("UiPageComponents")]
        public virtual UiComponent UiComponent { get; set; }
    }
}
