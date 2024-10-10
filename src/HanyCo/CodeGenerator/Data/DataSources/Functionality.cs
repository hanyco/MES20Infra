using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace HanyCo.Infra.Internals.Data.DataSources;

[Table("Functionality", Schema = "infra")]
[Index("DeleteCommandId", Name = "IX_Functionality_DeleteCommandId")]
[Index("GetAllQueryId", Name = "IX_Functionality_GetAllQueryId")]
[Index("GetByIdQueryId", Name = "IX_Functionality_GetByIdQueryId")]
[Index("InsertCommandId", Name = "IX_Functionality_InsertCommandId")]
[Index("ModuleId", Name = "IX_Functionality_ModuleId")]
[Index("SourceDtoId", Name = "IX_Functionality_SourceDtoId")]
[Index("UpdateCommandId", Name = "IX_Functionality_UpdateCommandId")]
public partial class Functionality
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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

    public long? ControllerId { get; set; }

    [ForeignKey("ControllerId")]
    [InverseProperty("Functionalities")]
    public virtual Controller? Controller { get; set; } = null!;

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

    [ForeignKey("ModuleId")]
    [InverseProperty("Functionalities")]
    public virtual Module? Module { get; set; }

    [ForeignKey("SourceDtoId")]
    [InverseProperty("Functionalities")]
    public virtual Dto SourceDto { get; set; } = null!;

    [ForeignKey("UpdateCommandId")]
    [InverseProperty("FunctionalityUpdateCommands")]
    public virtual CqrsSegregate UpdateCommand { get; set; } = null!;
}
