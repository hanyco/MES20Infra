using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Blazor.Components;

public sealed class BlazorCurrencyBox : HtmlSpan, IBlazorComponent
{
    private readonly HtmlInput _htmlInput;
    public BlazorCurrencyBox(string? id = null, string? name = null, string? currencySymbol = null, string? labelPrefix = null)
        : base(id, name, null, labelPrefix)
    {
        this.AddRowDiv = false;
        this.CurrencySymbol = currencySymbol ?? "$";
        this._htmlInput = new HtmlInput(InputType.Number);
        this.Children.Add(this._htmlInput);
        this.Step = 0.1f;
        this.Minimum = 0;
    }

    public string? CurrencySymbol { get => this.Body; set => this.Body = value; }
    public float? Step { get => this._htmlInput.Step; set => this._htmlInput.Step = value; }
    public float? Minimum { get => this._htmlInput.Minimum; set => this._htmlInput.Minimum = value; }
    public float? Maximum { get => this._htmlInput.Maximum; set => this._htmlInput.Maximum = value; }
    public string? NameSpace { get; }
}