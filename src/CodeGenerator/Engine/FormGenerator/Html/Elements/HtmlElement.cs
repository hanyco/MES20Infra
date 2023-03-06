using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements
{
    public sealed class HtmlElement : HtmlElementBase<HtmlElement>, IHtmlElement
    {
        public HtmlElement(string tagName, string? id = null, string? name = null, string? body = null, string? prefix = null)
            : base(tagName, id, name, body, prefix)
        {
        }
    }
}
