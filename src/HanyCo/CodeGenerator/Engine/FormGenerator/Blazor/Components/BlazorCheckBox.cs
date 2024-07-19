using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements;
using HanyCo.Infra.CodeGeneration.Helpers;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Blazor.Components;

public sealed class BlazorCheckBox : HtmlElementBase<BlazorCheckBox>, IBlazorComponent
{
    public BlazorCheckBox(string? id = null, string? name = null, string? body = null, string? prefix = null, string? bind = null)
        : base("InputCheckbox", id, name, body, prefix) => this.SetBind(bind, "@bind-Value");

    public string? NameSpace { get; }
}