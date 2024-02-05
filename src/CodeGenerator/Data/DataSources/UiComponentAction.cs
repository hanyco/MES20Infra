using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HanyCo.Infra.Internals.Data.DataSources;

[Table("UiComponentAction", Schema = "infra")]
public partial class UiComponentAction
{
    /// <summary>
    /// مکان در کامپوننت
    /// </summary>
    [Key]
    public long Id { get; set; }

    public long UiComponentId { get; set; }

    public long? CqrsSegregateId { get; set; }

    public int TriggerTypeId { get; set; }

    [StringLength(1024)]
    public string? EventHandlerName { get; set; }

    public long PositionId { get; set; }

    [StringLength(1024)]
    public string? Caption { get; set; }

    public bool? IsEnabled { get; set; }

    [StringLength(1024)]
    public string? Name { get; set; }

    [ForeignKey("CqrsSegregateId")]
    [InverseProperty("UiComponentActions")]
    public virtual CqrsSegregate? CqrsSegregate { get; set; }

    [ForeignKey("PositionId")]
    [InverseProperty("UiComponentActions")]
    public virtual UiBootstrapPosition Position { get; set; } = null!;

    [ForeignKey("UiComponentId")]
    [InverseProperty("UiComponentActions")]
    public virtual UiComponent UiComponent { get; set; } = null!;
}
