using System;
using System.Collections.Generic;

namespace Application.Infrastructure.Persistence;

public partial class Module
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public Guid Guid { get; set; }

    public long? ParentId { get; set; }

    public virtual ICollection<Controller> Controllers { get; set; } = new List<Controller>();

    public virtual ICollection<CqrsSegregate> CqrsSegregates { get; set; } = new List<CqrsSegregate>();

    public virtual ICollection<Dto> Dtos { get; set; } = new List<Dto>();

    public virtual ICollection<Functionality> Functionalities { get; set; } = new List<Functionality>();

    public virtual ICollection<UiPage> UiPages { get; set; } = new List<UiPage>();
}
