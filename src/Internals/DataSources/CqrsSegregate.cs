using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace HanyCo.Infra.Internals.Data.DataSources
{
    [Table("CqrsSegregate", Schema = "infra")]
    [Index(nameof(ResultDtoId), Name = "IX_CqrsSegregate_ResultDtoId")]
    public partial class CqrsSegregate
    {
        public CqrsSegregate()
        {
            UiComponentActions = new HashSet<UiComponentAction>();
        }

        [Key]
        public long Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        public int SegregateType { get; set; }
        public string FriendlyName { get; set; }
        public string Description { get; set; }
        public long? ParamDtoId { get; set; }
        public long? ResultDtoId { get; set; }
        public Guid Guid { get; set; }
        public string Comment { get; set; }
        public long ModuleId { get; set; }
        public int CategoryId { get; set; }

        [ForeignKey(nameof(ModuleId))]
        [InverseProperty("CqrsSegregates")]
        public virtual Module Module { get; set; }
        [ForeignKey(nameof(ParamDtoId))]
        [InverseProperty(nameof(Dto.CqrsSegregateParamDtos))]
        public virtual Dto ParamDto { get; set; }
        [ForeignKey(nameof(ResultDtoId))]
        [InverseProperty(nameof(Dto.CqrsSegregateResultDtos))]
        public virtual Dto ResultDto { get; set; }
        [InverseProperty(nameof(UiComponentAction.CqrsSegregation))]
        public virtual ICollection<UiComponentAction> UiComponentActions { get; set; }
    }
}
