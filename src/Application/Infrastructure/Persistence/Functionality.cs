using System;
using System.Collections.Generic;

namespace Application.Infrastructure.Persistence;

public partial class Functionality
{
    public long Id { get; set; }

    public string? Name { get; set; }

    public long ModuleId { get; set; }

    public Guid Guid { get; set; }

    public string? Comment { get; set; }

    public long GetAllQueryId { get; set; }

    public long GetByIdQueryId { get; set; }

    public long InsertCommandId { get; set; }

    public long UpdateCommandId { get; set; }

    public long DeleteCommandId { get; set; }

    public long SourceDtoId { get; set; }

    public long ControllerId { get; set; }

    public virtual Controller Controller { get; set; } = null!;

    public virtual CqrsSegregate DeleteCommand { get; set; } = null!;

    public virtual CqrsSegregate GetAllQuery { get; set; } = null!;

    public virtual CqrsSegregate GetByIdQuery { get; set; } = null!;

    public virtual CqrsSegregate InsertCommand { get; set; } = null!;

    public virtual Module Module { get; set; } = null!;

    public virtual Dto SourceDto { get; set; } = null!;

    public virtual CqrsSegregate UpdateCommand { get; set; } = null!;
}
