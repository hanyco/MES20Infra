using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements;

using Library.CodeGeneration.Models;
using Library.ComponentModel;
using Library.Data.Models;

using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Blazor.Components;

public sealed class BlazorTable : HtmlTableBase<BlazorTable>, IBlazorComponent, IBindable
{
    private BlazorTableBody _Body;

    public BlazorTable()
        : base("table", null, null, null, null)
    {
    }

    public BlazorTable(string? id = null, string? name = null, string? labelPrefix = null, HtmlTableHead? head = null, BlazorTableBody? body = null, string? bind = null)
        : base("table", id, name, labelPrefix, head)
    {
        this.Body = body ?? new();
        _ = this.SetDataContextName(bind);
    }

    //public string? DataContextName { get => this.Body.DataContextName; set => this.SetDataContextName(value); }
    public BlazorTableDataTemplate DataTemplate { get; } = new();

    public HtmlTableRow DataTemplateRow => this.Body.DataTemplateRow;
    public string? NameSpace { get; }
    private new BlazorTableBody Body { get => this._Body ??= new(); set => this._Body = value; }

    public static BlazorTable New(string? id = null, string? name = null, string? labelPrefix = null, HtmlTableHead? head = null, BlazorTableBody? body = null, string? bind = null)
        => new(id, name, labelPrefix, head, body, bind);

    public void Bind()
    {
        this.Body.DataContextName = this.DataTemplate.DataContextName;
        this.Head.Children.Clear();
        this.Body.Children.Clear();
        this.Body.InnerText = string.Empty;
        foreach (var (caption, bindingPathOrElement) in this.DataTemplate.DataColumns)
        {
            this.Head.Children.Add(new() { InnerText = caption });
            this.Body.DataTemplateRow.Children.Add(new()
            {
                InnerText = bindingPathOrElement switch
                {
                    IHtmlElement element => element.GenerateUiCode()?.Statement,
                    IEnumerable<IHtmlElement> elements => elements.Select(x => x.GenerateUiCode()).ToCodes().ComposeAll(Environment.NewLine).Statement,
                    string bindingPath => $"@item.{bindingPath}",
                    _ => string.Empty,
                }
            });
        }
    }

    public override BlazorTable Reset()
    {
        _ = (this.Body?.Reset());
        return base.Reset();
    }

    public BlazorTable SetDataColumns(IEnumerable<DataColumnBindingInfo> bindings)
    {
        _ = this.DataTemplate.DataColumns.ClearAndAddRange(bindings);
        return this;
    }
    public BlazorTable AddDataColumns(IEnumerable<DataColumnBindingInfo> bindings)
    {
        _ = this.DataTemplate.DataColumns.AddRange(bindings);
        return this;
    }
    public BlazorTable SetDataContextName(string? value)
    {
        this.DataTemplate.DataContextName = value;
        return this;
    }

    protected override string? OnGenerateCodeStatement()
    {
        this.Children.Clear();
        _ = this.Children.AddRange(new IHtmlElement[] { this.Head, this.Body });
        return base.OnGenerateCodeStatement();
    }
}

public sealed class BlazorTableBody : HtmlTableBody, IBlazorComponent, IHasInnerText, IBindable
{
    public BlazorTableBody(string? labelPrefix = null, IEnumerable<HtmlTableRow>? rows = null)
        : base(labelPrefix, rows)
    {
    }

    public string? DataContextName { get; set; }
    public HtmlTableRow DataTemplateRow { get; } = new();
    public string? NameSpace { get; }

    public new void Bind()
    {
        var me = this.Cast().As<IHtmlElement>()!;
        me.Children.Clear();
        me.Children.Add(this.DataTemplateRow);
    }

    protected override HtmlTableBody OnCodeGenAddChildren(in StringBuilder statement)
    {
        if (!this.DataContextName.IsNullOrEmpty())
        {
            _ = statement.AppendLine($"@if ({this.DataContextName} is not null)".ToString());
            _ = statement.AppendLine($"  @foreach (var item in {this.DataContextName})".ToString())
                .AppendLine("  {");
        }
        _ = base.OnCodeGenAddChildren(statement);
        if (!this.DataContextName.IsNullOrEmpty())
        {
            _ = statement.AppendLine("  }");
        }

        return this;
    }
}

//public partial class BlazorTableDataTemplate
public class BlazorTableDataTemplate : NotifyPropertyChanged
{
    //public record struct DataColumn(string Caption, string? BindingPath);

    //[Library.SourceGenerator.Contracts.AutoNotifyAttribute]
    private string? _dataContextName;

    public ObservableHashSet<DataColumnBindingInfo> DataColumns { get; } = new();

    public string? DataContextName
    {
        get => this._dataContextName;
        set => this.SetProperty(ref this._dataContextName, value);
    }
}