using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements
{
    public class HtmlLabel : HtmlElementBase<HtmlLabel>, IHtmlElement
    {
        private bool _IsFormLabel;
        public HtmlLabel(string? id = null, string? name = null, string? body = null, string? prefix = null)
            : base("label", id, name, body, prefix)
        {
        }

        public bool IsFormLabel { get => this._IsFormLabel; set => this.SetAsFormLabel(value); }

        public HtmlLabel SetAsFormLabel(bool value = true)
        {
            if (this._IsFormLabel == value)
            {
                return this;
            }

            if (value)
            {
                this.CssClasses.Add("col-form-label");
            }
            else
            {
                this.CssClasses.Remove("col-form-label");
            }

            this._IsFormLabel = value;
            return this;
        }
    }
}
