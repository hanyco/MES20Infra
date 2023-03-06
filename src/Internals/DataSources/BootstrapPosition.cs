using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace HanyCo.Infra.Internals.Data.DataSources
{
    [Table("BootstrapPosition", Schema = "infra")]
    public partial class BootstrapPosition
    {
        public BootstrapPosition()
        {
            UiComponentActions = new HashSet<UiComponentAction>();
            UiComponentProperties = new HashSet<UiComponentProperty>();
            UiPageComponents = new HashSet<UiPageComponent>();
        }

        [Key]
        public long Id { get; set; }
        public int? Order { get; set; }
        public int? Row { get; set; }
        public int? Col { get; set; }
        public int? ColSpan { get; set; }

        [InverseProperty(nameof(UiComponentAction.Position))]
        public virtual ICollection<UiComponentAction> UiComponentActions { get; set; }
        [InverseProperty(nameof(UiComponentProperty.Position))]
        public virtual ICollection<UiComponentProperty> UiComponentProperties { get; set; }
        [InverseProperty(nameof(UiPageComponent.Position))]
        public virtual ICollection<UiPageComponent> UiPageComponents { get; set; }
    }
}
