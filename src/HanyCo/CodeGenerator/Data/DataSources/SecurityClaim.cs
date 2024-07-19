using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HanyCo.Infra.Internals.Data.DataSources;

[Table("SecurityClaim", Schema = "infra")]
public partial class SecurityClaim
{
    [Key]
    public Guid Id { get; set; }

    [StringLength(50)]
    public string Key { get; set; } = null!;

    public Guid? Parent { get; set; }

    [InverseProperty("Claim")]
    public virtual ICollection<EntityClaim> EntityClaims { get; set; } = new List<EntityClaim>();

    [InverseProperty("Claim")]
    public virtual ICollection<UserClaimAccess> UserClaimAccesses { get; set; } = new List<UserClaimAccess>();
}
