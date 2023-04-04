using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

using Library.ComponentModel;
using Library.Interfaces;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements;

public class HtmlTable : HtmlTableBase<HtmlTable>
{
    public HtmlTable(string? id = null, string? name = null, string? labelPrefix = null, HtmlTableHead? head = null, HtmlTableBody? body = null)
        : base("table", id, name, labelPrefix, head)
        => this.Body = body ?? new();

    public new HtmlTableBody Body { get; set; }
}

public class HtmlTableBody : HtmlElementBase<HtmlTableBody>, IHtmlElement, IParent<HtmlTableRow>, IHasInnerText, IResetable<HtmlTableBody>, IBindable
{
    public HtmlTableBody(string? labelPrefix = null, IEnumerable<HtmlTableRow>? rows = null)
        : base("tbody", null, null, null, labelPrefix)
    {
        if (rows?.Any() == true)
        {
            _ = this.Children.AddRange(rows);
        }
        this.AddRowDiv = false;
    }

    public new IList<HtmlTableRow> Children { get; } = new List<HtmlTableRow>();

    public string? InnerText { get; set; }

    public void Bind()
    {
        var me = this.CastAs<IHtmlElement>()!;
        me.Children.Clear();
        _ = me.Children.AddRange(this.Children);
    }

    public virtual HtmlTableBody Reset()
    {
        this.Children.Clear();
        this.InnerText = string.Empty;
        return this;
    }
}

public class HtmlTableCell : HtmlElementBase<HtmlTableCell>, IHtmlElement, IHasInnerText, IResetable<HtmlTableCell>, IBindable
{
    public HtmlTableCell(string? id = null, string? name = null, string? body = null, string? labelPrefix = null)
        : base("td", id, name, body, labelPrefix) => this.AddRowDiv = false;

    public string? InnerText { get; set; }

    public void Bind()
        => this.Body = this.InnerText;

    public virtual HtmlTableCell Reset()
        => this.Fluent(() => this.InnerText = string.Empty);
}

public class HtmlTableCellHead : HtmlElementBase<HtmlTableCellHead>, IHtmlElement, IHasInnerText, IResetable<HtmlTableCellHead>, IBindable
{
    public HtmlTableCellHead(string? innerText = null, string? labelPrefix = null)
        : base("th", null, null, null, labelPrefix)
        => (this.InnerText, this.AddRowDiv) = (innerText, false);

    public string? InnerText { get; set; }

    public void Bind()
        => this.Body = this.InnerText;

    public virtual HtmlTableCellHead Reset()
        => this.Fluent(() => this.InnerText = string.Empty);
}

public class HtmlTableHead : HtmlElementBase<HtmlTableHead>, IHtmlElement, IParent<HtmlTableCellHead>, IResetable<HtmlTableHead>, IBindable
{
    public HtmlTableHead(string? labelPrefix = null, IEnumerable<HtmlTableCellHead>? head = null)
        : base("thead", null, null, null, labelPrefix)
    {
        if (head?.Any() == true)
        {
            _ = this.Children.AddRange(head);
        }
        this.AddRowDiv = false;
    }

    public new IList<HtmlTableCellHead> Children { get; } = new List<HtmlTableCellHead>();

    public void Bind()
    {
        var me = this.CastAs<IHtmlElement>()!;
        me.Children.Clear();
        var row = new HtmlTableRow();
        _ = row.Heads.AddRange(this.Children);
        me.Children.Add(row);
    }

    public virtual HtmlTableHead Reset()
        => this.Fluent(this.Children.Clear);

    protected override string OnGenerateCodeStatement()
    {
        this.Children.Clear();
        return base.OnGenerateCodeStatement();
    }
}

public class HtmlTableRow : HtmlElementBase<HtmlTableRow>, IHtmlElement, IParent<HtmlTableCell>, IResetable<HtmlTableRow>, IBindable
{
    public HtmlTableRow(string? labelPrefix = null, IEnumerable<HtmlTableCell>? children = null)
        : base("tr", null, null, null, labelPrefix)
    {
        if (children?.Any() == true)
        {
            _ = this.Children.AddRange(children);
        }
        this.AddRowDiv = false;
    }

    public new IList<HtmlTableCell> Children { get; } = new List<HtmlTableCell>();

    internal IList<HtmlTableCellHead> Heads { get; } = new List<HtmlTableCellHead>();

    public void Bind()
    {
        var me = this.CastAs<IHtmlElement>()!;
        me.Children.Clear();
        _ = me.Children.AddRange(this.Heads);
        _ = me.Children.AddRange(this.Children);
    }

    public virtual HtmlTableRow Reset()
    {
        this.Children.Clear();
        this.Heads?.Clear();
        return this;
    }
}