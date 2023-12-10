using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements;
using HanyCo.Infra.CodeGeneration.Helpers;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Blazor.Components;

public sealed class BlazorTextBox : HtmlElementBase<BlazorTextBox>, IBlazorComponent
{
    public BlazorTextBox(string? id = null, string? name = null, string? body = null, string? prefix = null, string? bind = null)
        : base("InputText", id, name, body, prefix)
    {
        this.SetBind(bind, "@bind-Value");
        this.Attributes.Add("type", "text");
    }

    public string? NameSpace { get; }
}