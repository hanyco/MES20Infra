using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace HanyCo.Infra.Internals.Data.DataSources
{
    [Table("UiPage", Schema = "infra")]
    public partial class UiPage
    {
        public UiPage()
        {
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
        public long ModuleId { get; set; }

        [InverseProperty(nameof(UiPageComponent.Page))]
        public virtual ICollection<UiPageComponent> UiPageComponents { get; set; }
    }
}
