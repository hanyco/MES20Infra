using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HanyCo.Infra.Internals.Data.DataSources;

[Table("SystemMenu", Schema = "infra")]
public partial class SystemMenu
{
    [Key]
    public long Id { get; set; }

    public long? ParentId { get; set; }

    public Guid Guid { get; set; }

    [StringLength(1024)]
    public string Caption { get; set; } = null!;

    public string? Uri { get; set; }
}
