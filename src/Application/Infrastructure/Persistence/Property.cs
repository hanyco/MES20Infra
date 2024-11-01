using System;
using System.Collections.Generic;

namespace Application.Infrastructure.Persistence;

public partial class Property
{
    public long Id { get; set; }

    public long ParentEntityId { get; set; }

    public int PropertyType { get; set; }

    public string? TypeFullName { get; set; }

    public string Name { get; set; } = null!;

    public bool? HasSetter { get; set; }

    public bool? HasGetter { get; set; }

    public bool? IsList { get; set; }

    public bool? IsNullable { get; set; }

    public string? Comment { get; set; }

    public string? DbObjectId { get; set; }

    public Guid Guid { get; set; }

    public long? DtoId { get; set; }

    public virtual Dto? Dto { get; set; }

    public virtual ICollection<UiComponentProperty> UiComponentProperties { get; set; } = new List<UiComponentProperty>();

    public virtual ICollection<UiComponent> UiComponents { get; set; } = new List<UiComponent>();
}
