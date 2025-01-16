using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace HanyCo.Infra.Internals.Data.DataSources;

[Table("Controller", Schema = "infra")]
public partial class Controller
{
    [Key]
    public long Id { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string ControllerName { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string? ControllerRoute { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? NameSpace { get; set; }

    public bool? IsAnonymousAllow { get; set; }

    public string? AdditionalUsings { get; set; }

    public string? CtorParams { get; set; }

    public string? Permission { get; set; }

    public long ModuleId { get; set; }

    [InverseProperty("Controller")]
    public virtual ICollection<ControllerMethod> ControllerMethods { get; set; } = new List<ControllerMethod>();

    [InverseProperty("Controller")]
    public virtual ICollection<Functionality> Functionalities { get; set; } = new List<Functionality>();

    [ForeignKey("ModuleId")]
    [InverseProperty("Controllers")]
    public virtual Module Module { get; set; } = null!;
}
