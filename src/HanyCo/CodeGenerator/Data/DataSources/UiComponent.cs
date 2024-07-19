using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HanyCo.Infra.Internals.Data.DataSources;

[Table("UiComponent", Schema = "infra")]
public partial class UiComponent
{
    [Key]
    public long Id { get; set; }

    [StringLength(50)]
    public string Name { get; set; } = null!;

    public Guid Guid { get; set; }

    public bool? IsEnabled { get; set; }

    public string? Caption { get; set; }

    [StringLength(50)]
    public string ClassName { get; set; } = null!;

    public string NameSpace { get; set; } = null!;

    public long? PageDataContextId { get; set; }

    public long? PageDataContextPropertyId { get; set; }

    public bool? IsGrid { get; set; }

    [ForeignKey("PageDataContextId")]
    [InverseProperty("UiComponents")]
    public virtual Dto? PageDataContext { get; set; }

    [ForeignKey("PageDataContextPropertyId")]
    [InverseProperty("UiComponents")]
    public virtual Property? PageDataContextProperty { get; set; }

    [InverseProperty("UiComponent")]
    public virtual ICollection<UiComponentAction> UiComponentActions { get; set; } = new List<UiComponentAction>();

    [InverseProperty("UiComponent")]
    public virtual ICollection<UiComponentProperty> UiComponentProperties { get; set; } = new List<UiComponentProperty>();

    [InverseProperty("UiComponent")]
    public virtual ICollection<UiPageComponent> UiPageComponents { get; set; } = new List<UiPageComponent>();
}
