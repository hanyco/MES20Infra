using System;
using System.Collections.Generic;

namespace Application.Infrastructure.Persistence;

public partial class Dto
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? NameSpace { get; set; }

    public long? ModuleId { get; set; }

    public string? DbObjectId { get; set; }

    public Guid Guid { get; set; }

    public string? Comment { get; set; }

    public bool IsParamsDto { get; set; }

    public bool IsResultDto { get; set; }

    public bool IsViewModel { get; set; }

    public bool? IsList { get; set; }

    public string? BaseType { get; set; }

    public virtual ICollection<CqrsSegregate> CqrsSegregateParamDtos { get; set; } = new List<CqrsSegregate>();

    public virtual ICollection<CqrsSegregate> CqrsSegregateResultDtos { get; set; } = new List<CqrsSegregate>();

    public virtual ICollection<Functionality> Functionalities { get; set; } = new List<Functionality>();

    public virtual Module? Module { get; set; }

    public virtual ICollection<Property> Properties { get; set; } = new List<Property>();

    public virtual ICollection<UiComponent> UiComponents { get; set; } = new List<UiComponent>();

    public virtual ICollection<UiPage> UiPages { get; set; } = new List<UiPage>();
}
