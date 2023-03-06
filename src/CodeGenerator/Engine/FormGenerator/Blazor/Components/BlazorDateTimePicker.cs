using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Blazor.Components;

public sealed class BlazorDateTimePicker : HtmlSpan, IBlazorComponent
{
    public BlazorDateTimePicker(string? id = null, string? name = null, string? body = null, string? labelPrefix = null)
        : base(id, name, body, labelPrefix)
    {
    }

    public string? NameSpace { get; }
}