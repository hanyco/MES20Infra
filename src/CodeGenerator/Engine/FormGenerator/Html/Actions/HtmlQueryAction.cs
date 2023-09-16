using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

using Library.CodeGeneration.Models;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Html.Actions;

public record HtmlCqrsAction : ISegregationAction
{
    public HtmlCqrsAction(string name, ICqrsSegregation segregation)
    {
        this.Segregation = segregation;
        this.Name = name;
    }

    public string Name { get; }
    public ICqrsSegregation Segregation { get; }
}

public record CommandCqrsSegregation(string Name, MethodArgument? Parameter = null, MethodArgument? Result = null) : ICommandCqrsSegregation;
public record QueryCqrsSegregation(string Name, MethodArgument? Parameter, MethodArgument? Result) : IQueryCqrsSegregation;