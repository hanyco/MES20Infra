using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

using Library.CodeGeneration.Models;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Html.Actions;

public record HtmlQueryAction : IHtmlAction
{
    public HtmlQueryAction(string name, IQueryCqrsSergregation segregation)
    {
        this.Name = name;
        this.Segregation = segregation;
    }

    public string Name { get; }
    public IQueryCqrsSergregation Segregation { get; }
    ICqrsSegregation IHtmlAction.Segregation => this.Segregation;
}

public record HtmlCommandAction : IHtmlAction
{
    public HtmlCommandAction(string name, ICommandCqrsSergregation segregation)
    {
        this.Segregation = segregation;
        this.Name = name;
    }

    public string Name { get; }
    public ICommandCqrsSergregation Segregation { get; }
    ICqrsSegregation IHtmlAction.Segregation => this.Segregation;
}

public record CommandCqrsSergregation(string Name, MethodArgument? Parameter = null, MethodArgument? Result = null)
    : ICommandCqrsSergregation;
public record QueryCqrsSergregation(string Name, MethodArgument? Parameter, MethodArgument? Result)
    : IQueryCqrsSergregation;