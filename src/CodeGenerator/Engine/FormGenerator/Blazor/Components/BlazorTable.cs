using System.Collections.ObjectModel;

using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements;

using Library.CodeGeneration.Models;
using Library.ComponentModel;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Blazor.Components;

public sealed class BlazorTable : HtmlTableBase<BlazorTable>, IBlazorComponent
{
    public ObservableCollection<BlazorTableRowAction> Actions { get; } = new ObservableCollection<BlazorTableRowAction>();
    Dictionary<string, string?> IBlazorComponent.BlazorAttributes { get; } = null!;
    public ObservableCollection<BlazorTableColumn> Columns { get; } = new ObservableCollection<BlazorTableColumn>();
    public string? DataContextName { get; set; }
    string? IBlazorComponent.NameSpace { get; }

    protected override string OnGenerateCodeStatement()
    {
        var buffer = new StringBuilder();
        _ = buffer.AppendLine("<table class=\"table\">")
            .AppendLine($"{HtmlDoc.INDENT}<thead>")
            .AppendLine($"{HtmlDoc.INDENT.Repeat(2)}<tr>");
        foreach (var column in this.Columns)
        {
            _ = buffer.AppendLine($"{HtmlDoc.INDENT.Repeat(3)}<th>{column.Title}</th>");
        }
        foreach (var action in this.Actions)
        {
            _ = buffer.AppendLine($"{HtmlDoc.INDENT.Repeat(3)}<th>{action.Title}</th>");
        }
        _ = buffer.AppendLine($"{HtmlDoc.INDENT.Repeat(2)}</tr>")
            .AppendLine($"{HtmlDoc.INDENT}</thead>");

        _ = buffer.AppendLine($"{HtmlDoc.INDENT}<tbody>");
        if (!this.DataContextName.IsNullOrEmpty())
        {
            _ = buffer.AppendLine($"{HtmlDoc.INDENT.Repeat(2)}@if ({this.DataContextName} is not null)");
            _ = buffer.AppendLine($"{HtmlDoc.INDENT.Repeat(3)}@foreach (var item in {this.DataContextName})")
                .AppendLine($"{HtmlDoc.INDENT.Repeat(3)}{{");
            _ = buffer.AppendLine($"{HtmlDoc.INDENT.Repeat(2)}<tr>");
            foreach (var column in this.Columns)
            {
                _ = buffer.AppendLine($"{HtmlDoc.INDENT.Repeat(3)}<td>@item.{column.BindingName}</td>");
            }
            foreach (var action in this.Actions)
            {
                var onClick = action.OnClick.IsNullOrEmpty()
                    ? $"\"@(() => {action.OnClick})\""
                    : $"\"@(() => this.{action.Name}_OnClick(item.Id))\"";
                _ = buffer.Append($"{HtmlDoc.INDENT.Repeat(3)}<td>")
                    .Append($"<button id=\"{this.Name}\" name=\"{this.Name}\" ")
                    .Append($"@onclick={onClick}>{action.Title}")
                    .Append("</button>")
                    .Append("</td>")
                    .AppendLine();
            }
            _ = buffer.AppendLine()
                .AppendLine($"{HtmlDoc.INDENT.Repeat(2)}</tr>");
            _ = buffer.AppendLine($"{HtmlDoc.INDENT.Repeat(3)}}}");
        }
        _ = buffer.AppendLine($"{HtmlDoc.INDENT}</tbody>");
        _ = buffer.AppendLine("</table>");
        return buffer.ToString();
    }
}

public sealed class BlazorTableColumn(string bindingName, string title) : NotifyPropertyChanged
{
    private string _bindingName = bindingName;
    private string _title = title;
    public string BindingName { get => this._bindingName; set => this.SetProperty(ref this._bindingName, value); }
    public string Title { get => this._title; set => this.SetProperty(ref this._title, value); }
}

public sealed class BlazorTableRowAction(string name, string title) : NotifyPropertyChanged
{
    private string? _onClick;
    private string _title = title;
    private string _name = name;
    public string? OnClick { get => this._onClick; set => this.SetProperty(ref this._onClick, value); }
    public string Title { get => this._title; set => this.SetProperty(ref this._title, value); }
    public string Name { get => this._name; set => this.SetProperty(ref this._name, value); }
}