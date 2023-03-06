using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace HanyCo.Infra.Internals.Data.DataSources
{
    [Table("Dto", Schema = "infra")]
    [Index(nameof(ModuleId), Name = "IX_Dto_ModuleId")]
    public partial class Dto
    {
        public Dto()
        {
            CqrsSegregateParamDtos = new HashSet<CqrsSegregate>();
            CqrsSegregateResultDtos = new HashSet<CqrsSegregate>();
            UiComponents = new HashSet<UiComponent>();
        }

        [Key]
        public long Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        public long? ModuleId { get; set; }
        [StringLength(50)]
        public string DbObjectId { get; set; }
        public Guid Guid { get; set; }
        [StringLength(50)]
        public string Comment { get; set; }
        public string NameSpace { get; set; }

        [ForeignKey(nameof(ModuleId))]
        [InverseProperty("Dtos")]
        public virtual Module Module { get; set; }
        [InverseProperty(nameof(CqrsSegregate.ParamDto))]
        public virtual ICollection<CqrsSegregate> CqrsSegregateParamDtos { get; set; }
        [InverseProperty(nameof(CqrsSegregate.ResultDto))]
        public virtual ICollection<CqrsSegregate> CqrsSegregateResultDtos { get; set; }
        [InverseProperty(nameof(UiComponent.Dto))]
        public virtual ICollection<UiComponent> UiComponents { get; set; }
    }
}
