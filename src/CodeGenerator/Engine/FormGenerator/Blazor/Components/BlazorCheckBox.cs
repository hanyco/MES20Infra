using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements;
using HanyCo.Infra.CodeGeneration.Helpers;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Blazor.Components
{
    public sealed class BlazorCheckBox : HtmlInput, IBlazorComponent
    {
        public BlazorCheckBox(string? id = null, string? name = null, string? bind = null, string? prefix = null)
            : base(InputType.Checkbox, id, name, null, prefix)
            => this.SetBind(bind);

        public string? NameSpace { get; }
    }
}
