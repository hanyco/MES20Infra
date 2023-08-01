using System.Diagnostics.CodeAnalysis;

using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements;

using Library.ComponentModel;
using Library.Exceptions.Validations;
using Library.Validations;

namespace HanyCo.Infra.CodeGeneration.Helpers;

public static class HtmlElementExtensions
{
    public static TElement AddChild<TElement>(this TElement element, IHtmlElement child)
        where TElement : IHtmlElement
    {
        element.ArgumentNotNull(nameof(element)).Children.Add(child);
        return element;
    }

    public static TElement AddChild<TElement>(this TElement element, params IHtmlElement[] children)
        where TElement : IHtmlElement
    {
        Check.MustBeArgumentNotNull(element);
        if (children?.Any() is true)
        {
            foreach (var child in children)
            {
                element!.Children.Add(child);
            }
        }

        return element;
    }

    public static TElement AddNewLine<TElement>(this TElement element)
        where TElement : IHtmlElement
    {
        element.ArgumentNotNull(nameof(element)).Children.Add(HtmlBr.New());
        return element;
    }

    public static StringBuilder GenerateChildrenCode(this IEnumerable<IHtmlElement> children, StringBuilder statementBuilder, bool manageRow = true)
    {
        if (children?.Any() is not true)
        {
            return statementBuilder;
        }
        var childs = validatePositions(children);
        var orderedData = sortData(childs);
        createDetails(statementBuilder, manageRow, orderedData);

        return statementBuilder;

        static IEnumerable<IHtmlElement> validatePositions(IEnumerable<IHtmlElement> children)
            => children.ArgumentNotNull()
                       .Any(x => x?.Position.Order is not null and not 0) && children.Any(x => x?.Position.Row is not null and not 0)
                           ? throw new ValidationException()
                           : children;

        static IEnumerable<IHtmlElement> sortData(IEnumerable<IHtmlElement> children)
        {
            IOrderedEnumerable<IHtmlElement>? result = null;
            if (!children.Any(x => x?.Position.Order is not null and not 0) && !children.Any(x => x?.Position.Row is not null and not 0))
            {
                result = children.OrderBy(x => 1);
            }
            else if (children.Any(x => x.Position.Order is not null and not 0))
            {
                result = children.Where(x => x.Position.Order is not null and not 0)
                    .OrderBy(x => x.Position.Order)
                    .AddRangeImmuted(children.Where(x => x.Position.Order is null or 0))
                    .OrderBy(x => 1);
            }
            else if (children.Any(x => x.Position.Row is not null and not 0))
            {
                result = children.OrderBy(x => x.Position.Row);
            }

            return result.Compact();
        }

        static int setupChildCodeStatement(StringBuilder statement, ref int colIndex, ref int? lastRow, IHtmlElement child, bool addRowDiv)
        {
            var colSpan = child.Position.ColSpan is null or < 1 ? 1 : child.Position.ColSpan.Value;
            if (addRowDiv)
            {
                if (lastRow is -1)
                {
                    _ = statement.AppendLine("<div class='row'>");
                    lastRow = child.Position.Row;
                }
                else if ((colIndex + colSpan) > 12 || lastRow != child.Position.Row)
                {
                    _ = statement.AppendLine("</div>");
                    _ = statement.AppendLine("<div class='row'>");
                    colIndex = 0;
                    lastRow = child.Position.Row;
                }
            }
            _ = child.Position.SetColSpan(colSpan);
            _ = statement.AppendLine($"{HtmlDoc.INDENT}{child.GenerateUiCode().Statement}");
            return colSpan;
        }

        static void createDetails(StringBuilder statementBuilder, bool manageRow, IEnumerable<IHtmlElement> orderedData)
        {
            var colIndex = 0;
            int? lastRow = -1;

            foreach (var child in orderedData)
            {
                child.Cast().As<IBindable>()?.Bind();
                var colSpan = setupChildCodeStatement(statementBuilder, ref colIndex, ref lastRow, child, manageRow);
                colIndex += colSpan;
            }
            if (lastRow is not -1)
            {
                _ = statementBuilder.AppendLine("</div>");
            }
        }
    }

    public static string? GetAttribute<TElement>(this TElement element, string key)
        where TElement : IHtmlElement
    {
        Check.MustBeArgumentNotNull(element);
        Check.MustBeArgumentNotNull(key);

        var target = element.Attributes;
        return target.TryGetValue(key, out var value) ? value : null;
    }

    public static string? GetAttribute<TElement>(this TElement element, string key, bool isBlazorAttribute = true)
        where TElement : IBlazorComponent
    {
        Check.MustBeArgumentNotNull(element);
        Check.MustBeArgumentNotNull(key);
        var target = isBlazorAttribute ? element.BlazorAttributes : element.Attributes;
        return target.TryGetValue(key, out var value) ? value : null;
    }

    public static string? GetBind<TElement>(this TElement element)
        where TElement : IBlazorComponent
        => element.GetAttribute("bind", true);

    public static TElement RemoveAttribute<TElement>(this TElement element, string key)
        where TElement : IHtmlElement
    {
        Check.MustBeArgumentNotNull(element);
        Check.MustBeArgumentNotNull(key);

        if (!element.Attributes.ContainsKey(key))
        {
            _ = element.Attributes.Remove(key);
        }
        return element;
    }

    public static TElement SetAttribute<TElement>(this TElement element, string key, string? value)
        where TElement : IHtmlElement
    {
        _ = element.ArgumentNotNull(nameof(element)).Attributes.SetByKey(key.ArgumentNotNull(nameof(key)), value);
        return element;
    }

    public static TElement SetAttribute<TElement>(this TElement element, string key, string? value, bool isBlazorAttribute = true)
        where TElement : IBlazorComponent
    {
        Check.MustBeArgumentNotNull(element);
        Check.MustBeArgumentNotNull(key);
        var target = isBlazorAttribute ? element.BlazorAttributes : element.Attributes;
        _ = target.SetByKey(key.ArgumentNotNull(nameof(key)), value);
        return element;
    }

    public static TElement SetBind<TElement>(this TElement element, string? value)
        where TElement : IBlazorComponent
        => Fluent(element, () => _ = value is not null ? element.SetAttribute("bind", value, true) : element.RemoveAttribute("bind"));

    public static TElement SetElementAttribute<TElement, TPropType>([DisallowNull] this TElement element, [DisallowNull] string key, TPropType? value)
        where TElement : notnull, IHtmlElement
        where TPropType : class => SetElementAttribute(element, key, value?.ToString(), value is not null);

    public static TElement SetElementAttribute<TElement>([DisallowNull] this TElement element, [DisallowNull] string key, string? value, bool mustSet)
        where TElement : notnull, IHtmlElement
    {
        Check.MustBeArgumentNotNull(element);
        Check.MustBeArgumentNotNull(key);
        if (mustSet)
        {
            _ = element.Attributes.SetByKey(key, value?.ToString());
        }
        else
        {
            _ = element.Attributes.Remove(key);
        }

        return element;
    }
}