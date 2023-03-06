using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace HanyCo.Infra.Internals.Data.DataSources
{
    [Table("UiComponent", Schema = "infra")]
    public partial class UiComponent
    {
        public UiComponent()
        {
            UiComponentProperties = new HashSet<UiComponentProperty>();
            UiPageComponents = new HashSet<UiPageComponent>();
        }

        [Key]
        public long Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        [StringLength(50)]
        public string ClassName { get; set; }
        public Guid Guid { get; set; }
        [Required]
        public string NameSpace { get; set; }
        public long DtoId { get; set; }
        public long Module { get; set; }

        [ForeignKey(nameof(DtoId))]
        [InverseProperty("UiComponents")]
        public virtual Dto Dto { get; set; }
        [InverseProperty(nameof(UiComponentProperty.UiComponent))]
        public virtual ICollection<UiComponentProperty> UiComponentProperties { get; set; }
        [InverseProperty(nameof(UiPageComponent.UiComponent))]
        public virtual ICollection<UiPageComponent> UiPageComponents { get; set; }
    }
}
