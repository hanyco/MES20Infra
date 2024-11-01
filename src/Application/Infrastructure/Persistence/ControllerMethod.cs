using System;
using System.Collections.Generic;

namespace Application.Infrastructure.Persistence;

public partial class ControllerMethod
{
    public long Id { get; set; }

    public long ControllerId { get; set; }

    public string Name { get; set; } = null!;

    public string? ReturnType { get; set; }

    public bool? IsAsync { get; set; }

    public string? Body { get; set; }

    public string? HttpMethods { get; set; }

    public string? Arguments { get; set; }

    public virtual Controller Controller { get; set; } = null!;
}
