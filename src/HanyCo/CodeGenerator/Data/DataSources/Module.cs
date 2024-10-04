using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace HanyCo.Infra.Internals.Data.DataSources;

[Table("Module", Schema = "infra")]
public partial class Module
{
    [Key]
    public long Id { get; set; }

    [StringLength(50)]
    public string Name { get; set; } = null!;

    public Guid Guid { get; set; }

    public long? ParentId { get; set; }

    [InverseProperty("Module")]
    public virtual ICollection<Controller> Controllers { get; set; } = new List<Controller>();

    [InverseProperty("Module")]
    public virtual ICollection<CqrsSegregate> CqrsSegregates { get; set; } = new List<CqrsSegregate>();

    [InverseProperty("Module")]
    public virtual ICollection<Dto> Dtos { get; set; } = new List<Dto>();

    [InverseProperty("Module")]
    public virtual ICollection<Functionality> Functionalities { get; set; } = new List<Functionality>();

    [InverseProperty("Module")]
    public virtual ICollection<UiPage> UiPages { get; set; } = new List<UiPage>();
}
