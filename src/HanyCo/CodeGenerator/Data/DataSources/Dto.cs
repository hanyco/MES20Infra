using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HanyCo.Infra.Internals.Data.DataSources;

[Table("Dto", Schema = "infra")]
[Index("ModuleId", Name = "IX_Dto_ModuleId")]
public partial class Dto
{
    [Key]
    public long Id { get; set; }

    [StringLength(50)]
    public string Name { get; set; } = null!;

    [StringLength(1024)]
    public string? NameSpace { get; set; }

    public long? ModuleId { get; set; }

    [StringLength(50)]
    public string? DbObjectId { get; set; }

    public Guid Guid { get; set; }

    [StringLength(50)]
    public string? Comment { get; set; }

    public bool IsParamsDto { get; set; }

    public bool IsResultDto { get; set; }

    public bool IsViewModel { get; set; }

    public bool? IsList { get; set; }

    [InverseProperty("ParamDto")]
    public virtual ICollection<CqrsSegregate> CqrsSegregateParamDtos { get; set; } = new List<CqrsSegregate>();

    [InverseProperty("ResultDto")]
    public virtual ICollection<CqrsSegregate> CqrsSegregateResultDtos { get; set; } = new List<CqrsSegregate>();

    [InverseProperty("SourceDto")]
    public virtual ICollection<Functionality> Functionalities { get; set; } = new List<Functionality>();

    [ForeignKey("ModuleId")]
    [InverseProperty("Dtos")]
    public virtual Module? Module { get; set; }

    [InverseProperty("Dto")]
    public virtual ICollection<Property> Properties { get; set; } = new List<Property>();

    [InverseProperty("PageDataContext")]
    public virtual ICollection<UiComponent> UiComponents { get; set; } = new List<UiComponent>();

    [InverseProperty("Dto")]
    public virtual ICollection<UiPage> UiPages { get; set; } = new List<UiPage>();
}
