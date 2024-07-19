using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.CodeGeneration.Helpers;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements;

public class HtmlFrame : HtmlElementBase<HtmlFrame>, IHtmlElement
{

    public HtmlFrame(string? id = null, string? name = null, string? labelPrefix = null)
        : base("iframe", id, name, null, labelPrefix)
    {

    }

    private string? _source;
    public string? Source
    {
        get => this._source;
        set
        {
            this._source = value;
            this.SetElementAttribute("src", value);
        }
    }

    private string? _title;
    public string? Title
    {
        get => this._title;
        set
        {
            this._title = value;
            this.SetElementAttribute("title", value);
        }
    }
}
