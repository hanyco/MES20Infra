using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.CodeGeneration.Helpers;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements;

public class HtmlSelect : HtmlElementBase<HtmlSelect>, IHtmlElement
{
    private bool _isAutoFocus;

    public List<HtmlOption> Options { get; } = new();

    public HtmlSelect(string? id = null, string? name = null, string? body = null, string? labelPrefix = null)
        : base("select", id, name, body, labelPrefix) => this.AddRowDiv = false;

    public bool IsAutoFocus
    {
        get => this._isAutoFocus;
        set
        {
            this._isAutoFocus = value;
            this.SetElementAttribute("autofocus", null, value);
        }
    }
    private string? _formId;
    public string? FormId
    {
        get => this._formId;
        set
        {
            this._formId = value;
            this.SetElementAttribute("form", value);
        }
    }

    private int? _size;
    public int? Size
    {
        get => this._size;
        set
        {
            this._size = value;
            this.SetElementAttribute("size", value?.ToString(), value is not null and not 0);
        }
    }

    private bool _isMultuple;

    public bool IsMultuple
    {
        get => this._isMultuple;
        set
        {
            this._isMultuple = value;
            this.SetElementAttribute("multiple", null, value);
        }
    }

    private bool _isRequired;

    public bool IsRequired
    {
        get => this._isRequired;
        set
        {
            this._isRequired = value;
            this.SetElementAttribute("required", null, value);
        }
    }

    protected override void OnGeneratingCode()
    {
        this.Children.Clear();
        this.Children.AddRange(this.Options.Cast<IHtmlElement>());
    }
}