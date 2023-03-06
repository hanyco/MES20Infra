using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.CodeGeneration.Helpers;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements;

public class HtmlInput : HtmlElementBase<HtmlInput>, IHtmlElement
{
    private InputType _type;
    public HtmlInput(InputType type, string? id = null, string? name = null, string? body = null, string? prefix = null)
        : base("input", id, name, body, prefix) => this.Type = type;

    public bool IsFormControl { get; private set; }

    public InputType Type
    {
        get => this._type;
        set
        {
            this.SetAttribute("type", value switch
            {
                InputType.Text => "text",
                InputType.Button => "button",
                InputType.DateTime => "datetime-local",
                InputType.Submit => "submit",
                InputType.Number => "number",
                InputType.Date => "date",
                InputType.Time => "time",
                InputType.Password => "password",
                InputType.Checkbox => "checkbox",
                _ => throw new NotImplementedException(),
            });
            this._type = value;
        }
    }

    public HtmlInput SetAsFormControl(bool value = true)
    {
        if (this.IsFormControl == value)
        {
            return this;
        }

        if (value)
        {
            this.CssClasses.Add("form-control");
        }
        else
        {
            this.CssClasses.Remove("form-control");
        }

        this.IsFormControl = value;
        return this;
    }

    private float? _minimum;
    public float? Minimum
    {
        get => this._minimum;
        set
        {
            this._minimum = value;
            this.SetElementAttribute("min", value?.ToString(), value is not null);
        }
    }

    private float? _maxinum;
    public float? Maximum
    {
        get => this._maxinum;
        set
        {
            this._maxinum = value;
            this.SetElementAttribute("max", value?.ToString(), value is not null);
        }
    }

    private float? _step;
    public float? Step
    {
        get => this._step;
        set
        {
            this._step = value;
            this.SetElementAttribute("step", value?.ToString(), value is not null);
        }
    }
}