using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace HanyCo.Infra.Internals.Data.DataSources;

[Table("EntityClaim", Schema = "infra")]
[Index("ClaimId", Name = "IX_EntityClaim_ClaimId")]
public partial class EntityClaim
{
    [Key]
    public Guid Id { get; set; }

    public Guid EntityId { get; set; }

    public Guid ClaimId { get; set; }

    [ForeignKey("ClaimId")]
    [InverseProperty("EntityClaims")]
    public virtual SecurityClaim Claim { get; set; } = null!;
}
