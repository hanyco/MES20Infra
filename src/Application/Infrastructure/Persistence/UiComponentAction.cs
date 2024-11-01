using System;
using System.Collections.Generic;

namespace Application.Infrastructure.Persistence;

public partial class UiComponentAction
{
    /// <summary>
    /// مکان در کامپوننت
    /// </summary>
    public long Id { get; set; }

    public long UiComponentId { get; set; }

    public long? CqrsSegregateId { get; set; }

    public int TriggerTypeId { get; set; }

    public string? EventHandlerName { get; set; }

    public long PositionId { get; set; }

    public string? Caption { get; set; }

    public bool? IsEnabled { get; set; }

    public string? Name { get; set; }

    public virtual CqrsSegregate? CqrsSegregate { get; set; }

    public virtual UiBootstrapPosition Position { get; set; } = null!;

    public virtual UiComponent UiComponent { get; set; } = null!;
}
