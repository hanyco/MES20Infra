using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements;
using HanyCo.Infra.CodeGeneration.Helpers;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Blazor.Components;

public sealed class BlazorLabel : HtmlLabel, IBlazorComponent
{
    public BlazorLabel(string? id = null, string? name = null, string? body = null, string? bind = null, string? prefix = null)
        : base(id, name, body, prefix) => this.SetBind(bind);

    public string? NameSpace { get; }
}