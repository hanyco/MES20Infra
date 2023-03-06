using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace HanyCo.Infra.Internals.Data.DataSources
{
    [Table("Property", Schema = "infra")]
    public partial class Property
    {
        public Property()
        {
            UiComponentProperties = new HashSet<UiComponentProperty>();
        }

        [Key]
        public long Id { get; set; }
        public long ParentEntityId { get; set; }
        public int PropertyType { get; set; }
        public string TypeFullName { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        public bool? HasSetter { get; set; }
        public bool? HasGetter { get; set; }
        public bool? IsList { get; set; }
        public bool? IsNullable { get; set; }
        public string Comment { get; set; }
        [StringLength(50)]
        public string DbObjectId { get; set; }
        public Guid Guid { get; set; }

        [InverseProperty(nameof(UiComponentProperty.Property))]
        public virtual ICollection<UiComponentProperty> UiComponentProperties { get; set; }
    }
}
