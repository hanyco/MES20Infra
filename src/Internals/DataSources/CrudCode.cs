using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace HanyCo.Infra.Internals.Data.DataSources
{
    [Table("CrudCode", Schema = "infra")]
    public partial class CrudCode
    {
        [Key]
        public long Id { get; set; }
        [Required]
        [StringLength(1024)]
        public string Name { get; set; }
        [StringLength(50)]
        public string DbObjectId { get; set; }
        public long? ModuleId { get; set; }
        public long? GetCqrsSegregateId { get; set; }
        public long? GetByIdCqrsSegregateId { get; set; }
        public long? CreateCqrsSegregateId { get; set; }
        public long? UpdateCqrsSegregateId { get; set; }
        public long? DeleteCqrsSegregateId { get; set; }
        [Required]
        public string CqrsCodeNameSpace { get; set; }
        [Required]
        public string DtoCodeNameSpace { get; set; }
        public Guid Guid { get; set; }

        [ForeignKey(nameof(ModuleId))]
        [InverseProperty("CrudCodes")]
        public virtual Module Module { get; set; }
    }
}
