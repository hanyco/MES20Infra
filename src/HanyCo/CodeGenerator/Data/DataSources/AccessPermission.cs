using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace HanyCo.Infra.Internals.Data.DataSources;

[Table("AccessPermissions", Schema = "infra")]
public partial class AccessPermission
{
    [Key]
    public long Id { get; set; }

    public long? ParentId { get; set; }

    [StringLength(50)]
    public string EntityType { get; set; } = null!;

    public long EntityId { get; set; }

    [StringLength(128)]
    public string UserId { get; set; } = null!;

    [StringLength(50)]
    public string? AccessType { get; set; }

    [StringLength(50)]
    public string AccessScope { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    public string? Comment { get; set; }
}
