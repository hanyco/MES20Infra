#pragma warning disable IDE0058 // Expression value is never used

using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.CodeGeneration.Helpers;

using Library.Helpers;
using Library.Mapping;
using System.Collections.ObjectModel;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements
{
    [Immutable]
    [Fluent]
    public sealed class HtmlDoc
    {
        public const string INDENT = "    ";
        public const string QUOTE_MARK = "\"";
        public string? Charset { get; set; } = "utf-8";

        public Collection<IHtmlElement> Children { get; } = new();

        public string? Dir { get; set; } = "rtl";

        public bool DocTypeEnabled { get; private set; } = true;

        public string? Language { get; set; } = "fa";

        public string? Title { get; set; }

        public static HtmlDoc New() => new();

        public string GenerateCodeStatement()
        {
            var statement = new StringBuilder();

            if (this.DocTypeEnabled)
            {
                statement.AppendLine("<!DOCTYPE html>");
                statement.AppendLine();
            }

            var html = new HtmlElement("html");
            if (!this.Language.IsNullOrEmpty())
            {
                html.SetAttribute("lang", this.Language);
            }
            if (!this.Dir.IsNullOrEmpty())
            {
                html.SetAttribute("dir", this.Dir);
            }
            html.SetAttribute("xmlns", "http://www.w3.org/1999/xhtml");

            var head = new HtmlElement("head");
            if (!this.Charset.IsNullOrEmpty())
            {
                var meta = new HtmlElement("meta");
                meta.SetAttribute("charset", this.Charset);
                head.AddChild(meta);
            }

            if (!this.Title.IsNullOrEmpty())
            {
                head.AddChild(new HtmlElement("title", body: this.Title));
            }
            html = html.AddChild(head);
            var body = new HtmlElement("body");
            foreach (var child in this.Children)
            {
                body.AddChild(child);
            }
            html.AddChild(body);

            statement.AppendLine(html.ToString());
            return statement.ToString();
        }

        public HtmlDoc SetCharset(in string? charset)
        {
            this.Charset = charset;
            return this;
        }

        public HtmlDoc SetDir(in string? dir)
        {
            this.Dir = dir;
            return this;
        }

        public HtmlDoc SetDocTypeEnabled(bool use = true) 
            => this.With(me => me.DocTypeEnabled = use);

        public HtmlDoc SetLanguage(in string? lang)
        {
            this.Language = lang;
            return this;
        }

        public HtmlDoc SetTitle(in string? title)
        {
            this.Title = title;
            return this;
        }

        public override string ToString() => this.GenerateCodeStatement();
    }
}
#pragma warning restore IDE0058 // Expression value is never used