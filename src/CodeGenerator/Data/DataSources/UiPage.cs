using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HanyCo.Infra.Internals.Data.DataSources;

[Table("UiPage", Schema = "infra")]
public partial class UiPage
{
    [Key]
    public long Id { get; set; }

    [StringLength(50)]
    public string Name { get; set; } = null!;

    [StringLength(50)]
    public string ClassName { get; set; } = null!;

    public Guid Guid { get; set; }

    public string NameSpace { get; set; } = null!;

    public long ModuleId { get; set; }

    public string? Route { get; set; }

    public long? DtoId { get; set; }

    [ForeignKey("DtoId")]
    [InverseProperty("UiPages")]
    public virtual Dto? Dto { get; set; }

    [ForeignKey("ModuleId")]
    [InverseProperty("UiPages")]
    public virtual Module Module { get; set; } = null!;

    [InverseProperty("Page")]
    public virtual ICollection<UiPageComponent> UiPageComponents { get; set; } = new List<UiPageComponent>();
}
