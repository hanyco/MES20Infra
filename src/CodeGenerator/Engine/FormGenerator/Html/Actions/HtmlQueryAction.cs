using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

using Library.CodeGeneration.Models;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Html.Actions;

public sealed class CqrsAction : ISegregationAction
{
    public CqrsAction(string name, ICqrsSegregation segregation)
    {
        this.Segregation = segregation;
        this.Name = name;
    }

    public string Name { get; }
    public ICqrsSegregation Segregation { get; }
}

public record CommandCqrsSegregation(string Name, MethodArgument? Parameter = null, MethodArgument? Result = null) : ICommandCqrsSegregation;
public record QueryCqrsSegregation(string Name, MethodArgument? Parameter, MethodArgument? Result) : IQueryCqrsSegregation;

public sealed class CustomAction : ICustomAction
{
    public CustomAction(string name, string? codeStatement)
    {
        this.Name = name;
        this.CodeStatement = codeStatement;
    }

    public string? CodeStatement { get; }
    public string Name { get; }
}