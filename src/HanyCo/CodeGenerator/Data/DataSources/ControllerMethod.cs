using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace HanyCo.Infra.Internals.Data.DataSources;

[Table("ControllerMethod", Schema = "infra")]
public partial class ControllerMethod
{
    [Key]
    public long Id { get; set; }

    public long ControllerId { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string? ReturnType { get; set; }

    public bool? IsAsync { get; set; }

    public string? Body { get; set; }

    public string? HttpMethods { get; set; }

    public string? Arguments { get; set; }

    [ForeignKey("ControllerId")]
    [InverseProperty("ControllerMethods")]
    public virtual Controller Controller { get; set; } = null!;
}
