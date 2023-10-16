using System.CodeDom;

using Library.CodeGeneration.Models;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Blazor.Actors;
public record PropertyActor(in string Type,
        in string Name,
        in string? Caption,
        in MemberAttributes? AccessModifier = null,
        in PropertyAccessor? Getter = null,
        in PropertyAccessor? Setter = null)
{
    public PropertyInfo ToPropertyInfo()
        => new(this.Type, this.Name, this.AccessModifier, this.Getter, this.Setter);
}

public class MethodActor(
    string? name,
    bool? isPartial = null,
    string? body = null,
    string? returnType = null,
    MemberAttributes? accessModifier = null,
    MethodArgument[]? arguments = null)
{
    public MemberAttributes? AccessModifier { get; } = accessModifier;
    public MethodArgument[]? Arguments { get; } = arguments;
    public string? Body { get; } = body;
    public bool? IsPartial { get; } = isPartial;
    public string? Name { get; } = name;
    public string? ReturnType { get; } = returnType;
}

public class ButtonActor(
    string? name,
    bool showOnGrid = false,
    string? caption = null,
    bool isPartial = false,
    string? body = null,
    string? returnType = null,
    MemberAttributes? accessModifier = null,
    MethodArgument[]? arguments = null,
    string? eventHandlerName = null) : MethodActor(name, isPartial, body, returnType, accessModifier, arguments)
{
    public string? Caption { get; } = caption;
    public string? EventHandlerName { get; } = eventHandlerName;
    public bool ShowOnGrid { get; } = showOnGrid;
}