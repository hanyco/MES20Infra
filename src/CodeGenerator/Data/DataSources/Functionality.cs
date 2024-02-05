using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HanyCo.Infra.Internals.Data.DataSources;

[Table("Functionality", Schema = "infra")]
public partial class Functionality
{
    [Key]
    public long Id { get; set; }

    [StringLength(50)]
    public string? Name { get; set; }

    public long? ModuleId { get; set; }

    public Guid Guid { get; set; }

    [StringLength(50)]
    public string? Comment { get; set; }

    public long GetAllQueryId { get; set; }

    public long GetByIdQueryId { get; set; }

    public long InsertCommandId { get; set; }

    public long UpdateCommandId { get; set; }

    public long DeleteCommandId { get; set; }

    public long SourceDtoId { get; set; }

    [ForeignKey("DeleteCommandId")]
    [InverseProperty("FunctionalityDeleteCommands")]
    public virtual CqrsSegregate DeleteCommand { get; set; } = null!;

    [ForeignKey("GetAllQueryId")]
    [InverseProperty("FunctionalityGetAllQueries")]
    public virtual CqrsSegregate GetAllQuery { get; set; } = null!;

    [ForeignKey("GetByIdQueryId")]
    [InverseProperty("FunctionalityGetByIdQueries")]
    public virtual CqrsSegregate GetByIdQuery { get; set; } = null!;

    [ForeignKey("InsertCommandId")]
    [InverseProperty("FunctionalityInsertCommands")]
    public virtual CqrsSegregate InsertCommand { get; set; } = null!;

    [ForeignKey("SourceDtoId")]
    [InverseProperty("Functionalities")]
    public virtual Dto SourceDto { get; set; } = null!;

    [ForeignKey("UpdateCommandId")]
    [InverseProperty("FunctionalityUpdateCommands")]
    public virtual CqrsSegregate UpdateCommand { get; set; } = null!;
}
