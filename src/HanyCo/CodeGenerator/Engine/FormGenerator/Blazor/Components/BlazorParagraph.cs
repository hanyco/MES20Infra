using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Blazor.Components
{
    public sealed class BlazorParagraph : HtmlParagraph, IBlazorComponent
    {
        public BlazorParagraph(string? id = null, string? name = null, string? body = null)
            : base(id, name, body)
        {
        }

        public string? NameSpace { get; }
    }
}
