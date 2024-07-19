using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.CodeGeneration.Helpers;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements
{
    public class HtmlDiv : HtmlElementBase<HtmlDiv>, IHtmlElement
    {
        private bool _IsRighAlign;
        private bool _IsRow;
        public HtmlDiv(string? id = null, string? name = null, string? body = null, string? prefix = null)
            : base("div", id, name, body, prefix) => this.AddCssClass("d-flex");

        public bool IsButtonBar { get; private set; }

        public bool IsContainerFluid { get; private set; }

        public bool IsForm { get; private set; }

        public bool IsRighAlign => this._IsRighAlign;

        public bool IsRightToLeft { get; private set; }

        public bool IsRow { get => this._IsRow; set => this.SetIsRow(value); }

        public static HtmlDiv New(string? id = null, string? name = null, string? body = null, string? prefix = null)
            => new(id, name, body, prefix);

        public HtmlDiv SetIsButtonBar(bool value = true)
        {
            if (this.IsButtonBar == value)
            {
                return this;
            }

            if (value)
            {
                this.CssClasses.Add("d-grid gap-2 d-md-block");
            }
            else
            {
                this.CssClasses.Remove("d-grid gap-2 d-md-block");
            }

            this.IsButtonBar = value;
            return this;
        }

        public HtmlDiv SetIsContainerFluid(bool value = true)
        {
            if (this.IsRightToLeft == value)
            {
                return this;
            }

            this.IsRightToLeft = value;
            return value ? this.SetAttribute("container-fluid", null) : this.RemoveAttribute("container-fluid");
        }

        public HtmlDiv SetIsForm(bool value = true)
        {
            if (this.IsForm == value)
            {
                return this;
            }

            if (value)
            {
                this.CssClasses.Add("mb-3");
            }
            else
            {
                this.CssClasses.Remove("mb-3");
            }

            this.IsForm = value;
            return this;
        }

        public void SetIsRighAlign(bool value)
        {
            if (this._IsRighAlign == value)
            {
                return;
            }

            if (value)
            {
                this.CssClasses.Add("flex-row-reverse");
            }
            else
            {
                this.CssClasses.Remove("flex-row-reverse");
            }

            this._IsRighAlign = value;
        }

        public HtmlDiv SetIsRightToLeft(bool value = true)
        {
            if (this.IsRightToLeft == value)
            {
                return this;
            }

            this.IsRightToLeft = value;
            return value ? this.SetAttribute("dir", "rtl") : this.RemoveAttribute("dir");
        }

        public HtmlDiv SetIsRow(bool value = true)
        {
            if (value && !this._IsRow)
            {
                this.CssClasses.Add("row");
            }
            else if (!value && this._IsRow)
            {
                this.CssClasses.Remove("row");
            }
            this._IsRow = value;
            return this;
        }
    }
}
