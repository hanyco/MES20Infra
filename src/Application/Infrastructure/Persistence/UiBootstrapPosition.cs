using System;
using System.Collections.Generic;

namespace Application.Infrastructure.Persistence;

public partial class UiBootstrapPosition
{
    public long Id { get; set; }

    public int? Order { get; set; }

    public int? Row { get; set; }

    public int? Col { get; set; }

    public int? ColSpan { get; set; }

    public virtual ICollection<UiComponentAction> UiComponentActions { get; set; } = new List<UiComponentAction>();

    public virtual ICollection<UiComponentProperty> UiComponentProperties { get; set; } = new List<UiComponentProperty>();

    public virtual ICollection<UiPageComponent> UiPageComponents { get; set; } = new List<UiPageComponent>();
}
