using System;
using System.Collections.Generic;

namespace HanyCo.Infra.Internals.Data.DataSources
{
    public partial class Translation
    {
        public long Id { get; set; }
        public string Key { get; set; } = null!;
        public string LangCode { get; set; } = null!;
        public string Value { get; set; } = null!;
    }
}
