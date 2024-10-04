using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HanyCo.Infra.Internals.Data.DataSources;

[Table("CqrsSegregate", Schema = "infra")]
[Index("ModuleId", Name = "IX_CqrsSegregate_ModuleId")]
[Index("ParamDtoId", Name = "IX_CqrsSegregate_ParamDtoId")]
[Index("ResultDtoId", Name = "IX_CqrsSegregate_ResultDtoId")]
public partial class CqrsSegregate
{
    [Key]
    public long Id { get; set; }

    [StringLength(50)]
    public string Name { get; set; } = null!;

    public string? CqrsNameSpace { get; set; }

    public int SegregateType { get; set; }

    public string? FriendlyName { get; set; }

    public string? Description { get; set; }

    public long ParamDtoId { get; set; }

    public long ResultDtoId { get; set; }

    public Guid Guid { get; set; }

    public string? Comment { get; set; }

    public long ModuleId { get; set; }

    public int CategoryId { get; set; }

    [InverseProperty("DeleteCommand")]
    public virtual ICollection<Functionality> FunctionalityDeleteCommands { get; set; } = new List<Functionality>();

    [InverseProperty("GetAllQuery")]
    public virtual ICollection<Functionality> FunctionalityGetAllQueries { get; set; } = new List<Functionality>();

    [InverseProperty("GetByIdQuery")]
    public virtual ICollection<Functionality> FunctionalityGetByIdQueries { get; set; } = new List<Functionality>();

    [InverseProperty("InsertCommand")]
    public virtual ICollection<Functionality> FunctionalityInsertCommands { get; set; } = new List<Functionality>();

    [InverseProperty("UpdateCommand")]
    public virtual ICollection<Functionality> FunctionalityUpdateCommands { get; set; } = new List<Functionality>();

    [ForeignKey("ModuleId")]
    [InverseProperty("CqrsSegregates")]
    public virtual Module Module { get; set; } = null!;

    [ForeignKey("ParamDtoId")]
    [InverseProperty("CqrsSegregateParamDtos")]
    public virtual Dto ParamDto { get; set; } = null!;

    [ForeignKey("ResultDtoId")]
    [InverseProperty("CqrsSegregateResultDtos")]
    public virtual Dto ResultDto { get; set; } = null!;

    [InverseProperty("CqrsSegregate")]
    public virtual ICollection<UiComponentAction> UiComponentActions { get; set; } = new List<UiComponentAction>();
}
