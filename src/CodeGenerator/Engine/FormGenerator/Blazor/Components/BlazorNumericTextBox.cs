using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements;
using HanyCo.Infra.CodeGeneration.Helpers;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Blazor.Components;

public sealed class BlazorNumericTextBox : HtmlElementBase<BlazorNumericTextBox>, IBlazorComponent
{
    public BlazorNumericTextBox(string? id = null, string? name = null, string? body = null, string? prefix = null, string? bind = null)
        : base("InputNumber", id, name, body, prefix) => this.SetBind(bind, "@bind-Value");

    public string? NameSpace { get; }
}