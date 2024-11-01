using System;
using System.Collections.Generic;

namespace Application.Infrastructure.Persistence;

public partial class UiComponent
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public Guid Guid { get; set; }

    public bool? IsEnabled { get; set; }

    public string? Caption { get; set; }

    public string ClassName { get; set; } = null!;

    public string NameSpace { get; set; } = null!;

    public long? PageDataContextId { get; set; }

    public long? PageDataContextPropertyId { get; set; }

    public bool? IsGrid { get; set; }

    public virtual Dto? PageDataContext { get; set; }

    public virtual Property? PageDataContextProperty { get; set; }

    public virtual ICollection<UiComponentAction> UiComponentActions { get; set; } = new List<UiComponentAction>();

    public virtual ICollection<UiComponentProperty> UiComponentProperties { get; set; } = new List<UiComponentProperty>();

    public virtual ICollection<UiPageComponent> UiPageComponents { get; set; } = new List<UiPageComponent>();
}
