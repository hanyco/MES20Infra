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
public record FieldActor()
{
    public bool IsPartial { get; internal set; }

    public FieldInfo ToFieldInfo() => new();
}
public record MethodActor(string? Name,
                          bool showOnGrid = false,
                          string? Caption = null,
                          bool IsPartial = false,
                          string? Body = null,
                          string? ReturnType = null,
                          MemberAttributes? AccessModifier = null,
                          MethodArgument[]? Arguments = null,
                          string? EventHandlerName = null)
{
    public MethodInfo ToMethodInfo() => new();
}