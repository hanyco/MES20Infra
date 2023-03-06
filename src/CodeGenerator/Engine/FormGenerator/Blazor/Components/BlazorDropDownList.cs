using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Blazor.Components;

public class BlazorDropDownList : HtmlSelect, IBlazorComponent
{
    public string? DataContextName { get; set; }
    public string? IdProperty { get; set; }
    public string? DisplayProperty { get; set; }
    public string? NameSpace { get; }

    protected override void OnGeneratingCode()
        => this.Children.Add(new HtmlOption($"@item.{this.IdProperty}", $"@item.{this.DisplayProperty}"));
    protected override BlazorDropDownList OnCodeGenAddChildren(in StringBuilder statement)
    {
        if (!this.DataContextName.IsNullOrEmpty())
        {
            statement.AppendLine($"@foreach (var item in {this.DataContextName})".ToString())
                .AppendLine("{");
        }
        base.OnCodeGenAddChildren(statement);
        if (!this.DataContextName.IsNullOrEmpty())
        {
            statement.AppendLine("}");
        }

        return this;
    }
}