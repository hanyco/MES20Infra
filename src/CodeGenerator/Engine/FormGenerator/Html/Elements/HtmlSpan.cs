using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements
{
    public class HtmlSpan : HtmlElementBase<HtmlSpan>, IHtmlElement
    {
        public HtmlSpan(string? id = null, string? name = null, string? body = null, string? labelPrefix = null)
            : base("span", id, name, body, labelPrefix)
        {
        }
    }

    public class HtmlParagraph : HtmlElementBase<HtmlParagraph>, IHtmlElement
    {
        public HtmlParagraph(string? id = null, string? name = null, string? body = null, string? labelPrefix = null)
            : base("p", id, name, body, labelPrefix)
        {
        }
    }
}
