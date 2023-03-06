using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace HanyCo.Infra.Internals.Data.DataSources
{
    [Table("Module", Schema = "infra")]
    public partial class Module
    {
        public Module()
        {
            CqrsSegregates = new HashSet<CqrsSegregate>();
            CrudCodes = new HashSet<CrudCode>();
            Dtos = new HashSet<Dto>();
        }

        [Key]
        public long Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        public Guid Guid { get; set; }
        public long? ParentId { get; set; }

        [InverseProperty(nameof(CqrsSegregate.Module))]
        public virtual ICollection<CqrsSegregate> CqrsSegregates { get; set; }
        [InverseProperty(nameof(CrudCode.Module))]
        public virtual ICollection<CrudCode> CrudCodes { get; set; }
        [InverseProperty(nameof(Dto.Module))]
        public virtual ICollection<Dto> Dtos { get; set; }
    }
}
