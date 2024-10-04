using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace HanyCo.Infra.Internals.Data.DataSources;

[Keyless]
[Table("Translation", Schema = "infra")]
public partial class Translation
{
    public long Id { get; set; }

    public string Key { get; set; } = null!;

    [StringLength(10)]
    public string LangCode { get; set; } = null!;

    public string Value { get; set; } = null!;
}
