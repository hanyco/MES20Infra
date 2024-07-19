using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements;

public class HtmlHr : HtmlElementBase<HtmlHr>, IHtmlElement
{
    public HtmlHr()
        : base("hr") => this.BootStrapCol = null;

    public static HtmlHr New()
        => new();
}