using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements;

public class HtmlScript : HtmlElementBase<HtmlScript>, IHtmlElement
{
    public HtmlScript(string? type = "text/javascript", string? statement = null, string? labelPrefix = null)
        : base("script", labelPrefix)
    {

    }
    public string? Statement
    {
        get => this.Body;
        set => this.Body = value;
    }

}