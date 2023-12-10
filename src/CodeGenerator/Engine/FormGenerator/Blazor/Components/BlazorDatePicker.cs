using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements;
using HanyCo.Infra.CodeGeneration.Helpers;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Blazor.Components
{
    public sealed class BlazorDatePicker : HtmlElementBase<BlazorDatePicker>, IBlazorComponent
    {
        public BlazorDatePicker(string? id = null, string? name = null, string? body = null, string? prefix = null, string? bind = null)
            : base("InputDate", id, name, body, prefix)
        {
            this.SetBind(bind, "@bind-Value");
            Attributes.Add("Type", "InputDateType.DateTimeLocal");
        }

        public string? NameSpace { get; }
    }
}
