using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.CodeGeneration.Helpers;

using Library.CodeGeneration.Models;
using Library.ComponentModel;
using Library.Validations;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements;

public abstract class HtmlElementBase<TSelf> : IEquatable<TSelf>, IUiCodeGenerator
        where TSelf : HtmlElementBase<TSelf>, IHtmlElement
{
    private bool _isEnabled;
    private string? _style;

    protected HtmlElementBase(string tagName, string? id = null, string? name = null, string? body = null, string? labelPrefix = null)
    {
        this.TagName = tagName;
        this.Id = id;
        this.Name = name ?? id;
        this.Body = body;
        this.LabelPrefix = labelPrefix;
    }

    public Dictionary<string, string?> Attributes { get; } = new();

    public Dictionary<string, string?> BlazorAttributes { get; } = new();

    public int? BootStrapCol
    {
        get => this.Position.Col;
        set
        {
            if (this.Position.Col != value)
            {
                this.Position = this.Position with
                {
                    Col = value
                };
            }
        }
    }

    public IList<IHtmlElement> Children { get; } = new List<IHtmlElement>();

    public List<string> CssClasses { get; } = new();

    public string? Id { get; init; }

    public bool IsEnabled { get => this._isEnabled; set => this.SetIsEnabled(value); }

    public virtual string? Name { get; init; }
    public BootstrapPosition Position { get; set; } = new BootstrapPosition();

    public string? Style
    {
        get => this._style;
        set
        {
            this._style = value;
            _ = this.This().SetElementAttribute("style", value);
        }
    }

    protected bool AddRowDiv { get; set; } = true;
    protected virtual string? Body { get; set; }

    protected virtual string? LabelPrefix { get; }

    /// <summary>Gets the full name of the normalized.</summary>
    /// <value>The full name of the normalized.</value>
    protected virtual string NormalizedFullName
        => this.LabelPrefix.IsNullOrEmpty()
                ? $"{this.TagName.ToLowerInvariant()}"
                : $"{this.LabelPrefix.ToLowerInvariant()}:{this.TagName.ToLowerInvariant()}";

    protected virtual string TagName { get; }

    public static bool operator !=(HtmlElementBase<TSelf> left, HtmlElementBase<TSelf> right)
        => !(left == right);

    public static bool operator ==(HtmlElementBase<TSelf> left, HtmlElementBase<TSelf> right)
        => left?.Equals(right) ?? false;

    public TSelf AddCssClass(string cssClass)
                => this.This().Fluent(() => this.CssClasses.Add(cssClass));

    public override bool Equals(object? obj)
        => obj switch
        {
            TSelf other => obj.GetType() == this.GetType() && other.TagName.EqualsTo(this.TagName),
            _ => false
        };

    public bool Equals(TSelf? other)
        => this.Equals(other?.Cast().As<object>());

    public virtual Code GenerateUiCode(in GenerateCodesParameters? arguments = null)
    {
        this.OnGeneratingCode();
        var statement = this.OnGenerateCodeStatement();
        return !statement.IsNullOrEmpty() ? new Code(this.TagName, Languages.BlazorCodeBehind, statement, false) : Code.Empty;
    }

    public override int GetHashCode()
        => HashCode.Combine(this.TagName, this.Id ?? this.Name);

    public TSelf SetIsEnabled(bool value)
    {
        if (this._isEnabled == value)
        {
            return this.This();
        }

        if (!value)
        {
            this.Attributes.Add("disabled", "disabled");
        }
        else
        {
            _ = this.Attributes.Remove("disabled");
        }
        this._isEnabled = value;
        return this.This();
    }

    public TSelf SetPosition(int? order = null, int? row = null, int? col = null, int? colSpan = null)
        => this.This().Fluent(() => this.Position = new BootstrapPosition(order, row, col, colSpan));

    public override string ToString()
        => $"ID: {this.Id ?? this.Name}, Type:{this.GetType().Name}";

    protected virtual TSelf CodeGenAddAttributes(in StringBuilder statement)
    {
        Check.MustBeArgumentNotNull(statement);
        foreach ((var key, var value) in this.Attributes!)
        {
            _ = value.IsNullOrEmpty() ? statement!.Append($" {key}") : statement.Append($" {key}='{value}'");
        }
        return this.This();
    }

    protected virtual TSelf CodeGenAddBlazorAttributes(in StringBuilder statement)
    {
        Check.MustBeArgumentNotNull(statement!);
        foreach (var blazorAttribute in this.BlazorAttributes)
        {
            var (key, value) = blazorAttribute;
            if (!key.StartsWith("@"))
            {
                key = $"@{key}";
            }
            _ = value.IsNullOrEmpty() ? statement!.Append($" {key}") : statement.Append($" {key}='{value}'");
        }
        return this.This();
    }

    protected virtual TSelf CodeGenAddBody(in StringBuilder statement)
    {
        if (!this.Body.IsNullOrEmpty())
        {
            _ = statement.ArgumentNotNull(nameof(statement)).Append(this.Body);
        }
        return this.This();
    }

    protected virtual TSelf CodeGenAddCssClasses(StringBuilder statement)
    {
        var cssClasses = string.Join(' ', this.CssClasses.ToArray());
        //! Concurrency
        var pos = this.Position;
        if (pos.ColSpan is not null and not 1)
        {
            cssClasses = $" col-{pos.ColSpan} {cssClasses}";
        }
        cssClasses = pos.Col switch
        {
            > 0 => $"col-{pos.Col} {cssClasses}",
            //x 0 or null => $"col {cssClasses}",
            0 or null => $"{cssClasses}",
            _ => null
        };

        if (!cssClasses.IsNullOrEmpty())
        {
            _ = statement.ArgumentNotNull(nameof(statement)).Append($" class='{cssClasses.Trim()}'");
        }

        return this.This()!;
    }

    protected virtual TSelf CodeGenAddId(in StringBuilder statement)
    {
        if (this.Id is not null)
        {
            _ = statement.ArgumentNotNull(nameof(statement)).Append($" id='{this.Id}'");
        }
        return this.This();
    }

    protected virtual TSelf CodeGenAddName(in StringBuilder statement)
    {
        if (this.Name is not null)
        {
            _ = statement.ArgumentNotNull(nameof(statement)).Append($" name='{this.Name}'");
        }
        return this.This();
    }

    protected TSelf CodeGenAppend(in StringBuilder statement, in string value)
    {
        _ = statement.ArgumentNotNull(nameof(statement)).Append(value);
        return this.This();
    }

    protected virtual TSelf CodeGenCloseElement(in StringBuilder statement)
        => this.CodeGenAppend(statement, $"</{this.TagName}>");

    protected virtual TSelf CodeGenCloseParentalTag(in StringBuilder statement)
        => this.CodeGenAppend(statement, $">");

    protected virtual TSelf CodeGenCloseSingleTag(in StringBuilder statement)
        => this.CodeGenAppend(statement, "/>");

    protected virtual TSelf CodeGenOpenTag(in StringBuilder statement)
        => this.CodeGenAppend(statement, $"<{this.TagName}");

    protected virtual TSelf OnCodeGenAddChildren(in StringBuilder statement)
    {
        if (statement is null)
        {
            throw new ArgumentNullException(nameof(statement));
        }

        _ = this.Children.GenerateChildrenCode(statement, manageRow: this.AddRowDiv);

        return this.This();
    }

    protected virtual string OnGenerateCodeStatement()
    {
        var statement = new StringBuilder();
        if (this is IBindable bindable)
        {
            bindable.Bind();
        }

        #region New way. But it's not done yet. 🥀
        ////var element = new Library.CodeGeneration.HtmlGeneration.HtmlElement(this.NormalizedFullName);
        ////if (this.Id is not null)
        ////{
        ////    _ = element.AddAttribute("id", this.Id);
        ////}

        ////if (this.Name is not null)
        ////{
        ////    _ = element.AddAttribute("name", this.Name);
        ////}

        ////var cssClasses = string.Join(' ', this.CssClasses.ToArray());
        //////! Concurrency
        ////var pos = this.Position;
        ////if (pos.ColSpan is not null and not 1)
        ////{
        ////    cssClasses = $" col-{pos.ColSpan} {cssClasses}";
        ////}
        ////cssClasses = pos.Col switch
        ////{
        ////    > 0 => $"col-{pos.Col} {cssClasses}",
        ////    0 or null => $"col {cssClasses}",
        ////    _ => null
        ////};

        ////if (!cssClasses.IsNullOrEmpty())
        ////{
        ////    _ = element.AddAttribute("class", cssClasses.Trim());
        ////}

        ////foreach ((var key, var value) in this.Attributes!)
        ////{
        ////    _ = element.AddAttribute(key, value);
        ////}

        ////foreach (var blazorAttribute in this.BlazorAttributes)
        ////{
        ////    var (key, value) = blazorAttribute;
        ////    if (!key.StartsWith("@"))
        ////    {
        ////        key = $"@{key}";
        ////    }
        ////    _ = element.AddAttribute(key, value);
        ////}
        ////var tmpCode = element.ToHtml(); 
        #endregion

        _ = this.CodeGenOpenTag(statement)
                .CodeGenAddId(statement)
                .CodeGenAddName(statement)
                .CodeGenAddCssClasses(statement)
                .CodeGenAddAttributes(statement)
                .CodeGenAddBlazorAttributes(statement);
        //! No child elements and no elements? OK. It's done.
        if (!this.Children.Any() && this.Body.IsNullOrEmpty())
        {
            _ = this.CodeGenCloseSingleTag(statement);
            return statement.ToString();
        }
        _ = this.CodeGenCloseParentalTag(statement);

        _ = this.CodeGenAddBody(statement);
        if (this.Children.Count != 0)
        {
            _ = statement.AppendLine();
            _ = this.OnCodeGenAddChildren(statement);
        }
        _ = this.CodeGenCloseElement(statement);
        return statement.ToString();
    }

    protected virtual void OnGeneratingCode()
    { }

    private TSelf This()
        => this.Cast().As<TSelf>()!;
}