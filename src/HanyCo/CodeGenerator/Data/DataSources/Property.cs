using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HanyCo.Infra.Internals.Data.DataSources;

[Table("Property", Schema = "infra")]
public partial class Property
{
    [Key]
    public long Id { get; set; }

    public long ParentEntityId { get; set; }

    public int PropertyType { get; set; }

    public string? TypeFullName { get; set; }

    [StringLength(50)]
    public string Name { get; set; } = null!;

    public bool? HasSetter { get; set; }

    public bool? HasGetter { get; set; }

    public bool? IsList { get; set; }

    public bool? IsNullable { get; set; }

    public string? Comment { get; set; }

    [StringLength(50)]
    public string? DbObjectId { get; set; }

    public Guid Guid { get; set; }

    public long? DtoId { get; set; }

    [ForeignKey("DtoId")]
    [InverseProperty("Properties")]
    public virtual Dto? Dto { get; set; }

    [InverseProperty("Property")]
    public virtual ICollection<UiComponentProperty> UiComponentProperties { get; set; } = new List<UiComponentProperty>();

    [InverseProperty("PageDataContextProperty")]
    public virtual ICollection<UiComponent> UiComponents { get; set; } = new List<UiComponent>();
}
