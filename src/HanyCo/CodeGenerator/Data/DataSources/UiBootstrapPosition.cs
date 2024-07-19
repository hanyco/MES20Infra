using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HanyCo.Infra.Internals.Data.DataSources;

[Table("UiBootstrapPosition", Schema = "infra")]
public partial class UiBootstrapPosition
{
    [Key]
    public long Id { get; set; }

    public int? Order { get; set; }

    public int? Row { get; set; }

    public int? Col { get; set; }

    public int? ColSpan { get; set; }

    [InverseProperty("Position")]
    public virtual ICollection<UiComponentAction> UiComponentActions { get; set; } = new List<UiComponentAction>();

    [InverseProperty("Position")]
    public virtual ICollection<UiComponentProperty> UiComponentProperties { get; set; } = new List<UiComponentProperty>();

    [InverseProperty("Position")]
    public virtual ICollection<UiPageComponent> UiPageComponents { get; set; } = new List<UiPageComponent>();
}
