using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

using Library.ComponentModel;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements
{
    public abstract class HtmlTableBase<THtmlTable> : HtmlElementBase<THtmlTable>, IHtmlElement, IResetable<THtmlTable>
        where THtmlTable : HtmlTableBase<THtmlTable>
    {
        protected HtmlTableBase(string tagName = "table", string? id = null, string? name = null, string? labelPrefix = null, HtmlTableHead? head = null)
            : base(tagName, id, name, labelPrefix)
            => (this.Head, this.AddRowDiv) = (head ?? new(), false);

        public HtmlTableHead Head { get; set; }

        public virtual THtmlTable Reset()
            => Fluent(this, () => this.Head.Reset()).CastAs<THtmlTable>()!;
    }
}