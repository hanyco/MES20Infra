using System;
using System.Collections.Generic;

namespace Application.Infrastructure.Persistence;

public partial class AccessPermission
{
    public long Id { get; set; }

    public long? ParentId { get; set; }

    public string EntityType { get; set; } = null!;

    public long EntityId { get; set; }

    public string UserId { get; set; } = null!;

    public string? AccessType { get; set; }

    public string AccessScope { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? Comment { get; set; }
}
