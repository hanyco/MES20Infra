using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace HanyCo.Infra.Internals.Data.DataSources;

[Table("UiComponentProperty", Schema = "infra")]
[Index("PositionId", Name = "IX_UiComponentProperty_PositionId")]
[Index("PropertyId", Name = "IX_UiComponentProperty_PropertyId")]
[Index("UiComponentId", Name = "IX_UiComponentProperty_UiComponentId")]
public partial class UiComponentProperty
{
    [Key]
    public long Id { get; set; }

    public long UiComponentId { get; set; }

    public long? CqrsSegregateId { get; set; }

    public long? PropertyId { get; set; }

    public int ControlTypeId { get; set; }

    /// <summary>
    /// مکان در کامپوننت
    /// </summary>
    public long PositionId { get; set; }

    [StringLength(50)]
    public string Caption { get; set; } = null!;

    public bool? IsEnabled { get; set; }

    public string? ControlExtraInfo { get; set; }

    [ForeignKey("PositionId")]
    [InverseProperty("UiComponentProperties")]
    public virtual UiBootstrapPosition Position { get; set; } = null!;

    [ForeignKey("PropertyId")]
    [InverseProperty("UiComponentProperties")]
    public virtual Property? Property { get; set; }

    [ForeignKey("UiComponentId")]
    [InverseProperty("UiComponentProperties")]
    public virtual UiComponent UiComponent { get; set; } = null!;
}
