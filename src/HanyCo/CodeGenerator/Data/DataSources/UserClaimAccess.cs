using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace HanyCo.Infra.Internals.Data.DataSources;

[Table("UserClaimAccess", Schema = "infra")]
[Index("ClaimId", Name = "IX_UserClaimAccess_ClaimId")]
public partial class UserClaimAccess
{
    [Key]
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid ClaimId { get; set; }

    public int AccessType { get; set; }

    [ForeignKey("ClaimId")]
    [InverseProperty("UserClaimAccesses")]
    public virtual SecurityClaim Claim { get; set; } = null!;
}
