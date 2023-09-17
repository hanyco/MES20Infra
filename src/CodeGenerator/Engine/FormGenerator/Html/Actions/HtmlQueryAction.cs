using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

using Library.CodeGeneration.Models;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Html.Actions;

public sealed class CqrsAction(string name, ICqrsSegregation segregation) : ISegregationAction
{
    public string Name { get; } = name;
    public ICqrsSegregation Segregation { get; } = segregation;
}

public sealed class CustomAction(string name, FormattableString? codeStatement) : ICustomAction
{
    public FormattableString? CodeStatement { get; } = codeStatement;
    public string Name { get; } = name;
}

public record CommandCqrsSegregation(string Name, MethodArgument? Parameter = null, MethodArgument? Result = null) : ICommandCqrsSegregation;
public record QueryCqrsSegregation(string Name, MethodArgument? Parameter, MethodArgument? Result) : IQueryCqrsSegregation;