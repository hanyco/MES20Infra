using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements;

public class HtmlBr : HtmlElementBase<HtmlBr>, IHtmlElement
{
    public HtmlBr()
        : base("br") => this.BootStrapCol = null;

    public static HtmlBr New()
        => new();

    public string? NameSpace { get; }
}
