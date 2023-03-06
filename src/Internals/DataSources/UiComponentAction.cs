using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace HanyCo.Infra.Internals.Data.DataSources
{
    [Table("UiComponentAction", Schema = "infra")]
    public partial class UiComponentAction
    {
        [Key]
        public long Id { get; set; }
        public long UiComponentId { get; set; }
        public long CqrsSegregationId { get; set; }
        public int TriggerTypeId { get; set; }
        public Guid Guid { get; set; }
        public string Caption { get; set; }
        public long PositionId { get; set; }

        [ForeignKey(nameof(CqrsSegregationId))]
        [InverseProperty(nameof(CqrsSegregate.UiComponentActions))]
        public virtual CqrsSegregate CqrsSegregation { get; set; }
        [ForeignKey(nameof(PositionId))]
        [InverseProperty(nameof(BootstrapPosition.UiComponentActions))]
        public virtual BootstrapPosition Position { get; set; }
    }
}
