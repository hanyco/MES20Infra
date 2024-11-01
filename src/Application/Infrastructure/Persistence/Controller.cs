using System;
using System.Collections.Generic;

namespace Application.Infrastructure.Persistence;

public partial class Controller
{
    public long Id { get; set; }

    public string ControllerName { get; set; } = null!;

    public string? ControllerRoute { get; set; }

    public string? NameSpace { get; set; }

    public bool? IsAnonymousAllow { get; set; }

    public string? AdditionalUsings { get; set; }

    public string? CtorParams { get; set; }

    public long ModuleId { get; set; }

    public virtual ICollection<ControllerMethod> ControllerMethods { get; set; } = new List<ControllerMethod>();

    public virtual ICollection<Functionality> Functionalities { get; set; } = new List<Functionality>();

    public virtual Module Module { get; set; } = null!;
}
