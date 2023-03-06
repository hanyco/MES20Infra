using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.CodeGeneration.Helpers;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements;

public class HtmlOption : HtmlElementBase<HtmlOption>, IHtmlElement
{
    private bool _isSelected;
    private string? _value;

    public HtmlOption(string? value, string? label = null, string? labelPrefix = null)
        : base("option ", labelPrefix: labelPrefix) => (this.Value, this.Label) = (value, label ?? value);

    public string? Value
    {
        get => this._value;
        set
        {
            this._value = value;
            this.SetElementAttribute("value", value);
        }
    }

    public string? Label { get => this.Body ?? this.Value; set => this.Body = value; }
    public bool IsSelected
    {
        get => this._isSelected; set
        {
            this._isSelected = value;
            if (value && !this.Attributes.ContainsKey("selected"))
            {
                this.Attributes.Add("selected", "selected");
            }
            else if (!value && this.Attributes.ContainsKey("selected"))
            {
                this.Attributes.Remove("selected");
            }
        }
    }
}
