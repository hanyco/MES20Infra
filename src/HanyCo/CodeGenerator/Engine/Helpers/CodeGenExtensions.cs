using HanyCo.Infra.CodeGeneration.CodeGenerator.Bases;
using HanyCo.Infra.CodeGeneration.CodeGenerator.Interfaces;

using Library.DesignPatterns.Markers;

namespace HanyCo.Infra.CodeGeneration.Helpers;

[Fluent]
public static class CodeGenExtensions
{
    public static T AddPartials<T>(this T t, Partials partials)
        where T : ISupportsPartiality
    {
        t.Partials = t.Partials.AddFlag(partials);
        return t;
    }

    /// <summary>
    ///     Adds the property.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propsContainer">The props container.</param>
    /// <param name="type">The type.</param>
    /// <param name="name">The name.</param>
    /// <param name="hasSetter">if set to <c>true</c> [has setter].</param>
    /// <param name="hasGetter">if set to <c>true</c> [has getter].</param>
    /// <param name="isList">if set to <c>true</c> [is list].</param>
    /// <returns></returns>
    public static T AddProp<T>(
        this T propsContainer,
        in CodeGenType type,
        in string name,
        bool isList = false,
        bool isNullable = false,
        bool hasGetter = true,
        bool hasSetter = true,
        string? comment = null)
        where T : IPropertyContainer
    {
        var result = CodeGenProp.New(type, name, isList, isNullable, hasSetter, hasGetter, comment);
        return propsContainer.AddProp(result);
    }

    public static T AddAttr<T>(
        this T attrsContainer,
        in string fullName, in CodeGenType? baseClass = null, in IEnumerable<CodeGenProp>? props = null, in IEnumerable<CodeGenAttr>? attributes = null)
        where T : IAttributeContainer
    {
        var result = new CodeGenAttr(fullName, baseClass, props, attributes);
        return attrsContainer.AddAttr(result);
    }

    /// <summary>
    ///     Adds the property.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propsContainer">The props container.</param>
    /// <param name="primitiveType">Type of the primitive.</param>
    /// <param name="name">The name.</param>
    /// <param name="hasSetter">if set to <c>true</c> [has setter].</param>
    /// <param name="comment">The comment.</param>
    /// <returns>
    ///     <br />
    /// </returns>
    public static T AddProp<T>(
        this T propsContainer,
        in Type primitiveType,
        in string name,
        bool hasSetter = true,
        string? comment = null,
        bool isList = false,
        bool isNullable = false)
        where T : IPropertyContainer
    {
        var result = CodeGenProp.New(new CodeGenType(primitiveType), name, isList, isNullable, true, hasSetter);
        if (comment is not null)
        {
            _ = result.SetSummary(comment);
        }

        return propsContainer.AddProp(result);
    }

    /// <summary>
    ///     Adds the property.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propsContainer">The props container.</param>
    /// <param name="prop">The property.</param>
    /// <returns></returns>
    public static T AddProp<T>(this T propsContainer, in CodeGenProp prop)
        where T : IPropertyContainer
    {
        propsContainer.Properties.Add(prop);
        return propsContainer;
    }

    public static T AddAttr<T>(this T attrContainer, in CodeGenAttr attr)
        where T : IAttributeContainer
    {
        attrContainer.Attributes.Add(attr);
        return attrContainer;
    }

    public static T ClearSummary<T>(this T t)
        where T : ISupportCommenting
    {
        t.Comment = null;
        return t;
    }

    public static T CreatePartialClass<T>(this T t)
        where T : ICodeGenCqrsSegregate
    {
        t.HasPartialClass = true;
        return t;
    }

    /// <summary>
    ///     Gets the base types.
    /// </summary>
    /// <param name="t">The t.</param>
    /// <returns></returns>
    public static IEnumerable<CodeGenType> GetBaseTypes(this ICanInherit t)
    {
        if (t.BaseClass is not null)
        {
            yield return t.BaseClass;
        }

        foreach (var @interface in t.Interfaces)
        {
            yield return @interface;
        }
    }

    public static T RemovePartialClass<T>(this T t)
        where T : ICodeGenCqrsSegregate
    {
        t.HasPartialClass = false;
        return t;
    }

    public static T SetSummary<T>(this T t, string comment)
        where T : ISupportCommenting
    {
        t.Comment = comment;
        return t;
    }
}
