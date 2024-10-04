using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace HanyCo.Infra.Internals.Data.DataSources;

[Table("UiPageComponent", Schema = "infra")]
[Index("PageId", Name = "IX_UiPageComponent_PageId")]
[Index("PositionId", Name = "IX_UiPageComponent_PositionId")]
[Index("UiComponentId", Name = "IX_UiPageComponent_UiComponentId")]
public partial class UiPageComponent
{
    [Key]
    public long Id { get; set; }

    public Guid Guid { get; set; }

    public long PageId { get; set; }

    public long UiComponentId { get; set; }

    /// <summary>
    /// مکان در پیج
    /// </summary>
    public long? PositionId { get; set; }

    [ForeignKey("PageId")]
    [InverseProperty("UiPageComponents")]
    public virtual UiPage Page { get; set; } = null!;

    [ForeignKey("PositionId")]
    [InverseProperty("UiPageComponents")]
    public virtual UiBootstrapPosition? Position { get; set; }

    [ForeignKey("UiComponentId")]
    [InverseProperty("UiPageComponents")]
    public virtual UiComponent UiComponent { get; set; } = null!;
}
