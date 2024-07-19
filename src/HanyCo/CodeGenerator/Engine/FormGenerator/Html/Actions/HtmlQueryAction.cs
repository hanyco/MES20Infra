using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

using Library.CodeGeneration.Models;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Html.Actions;

public sealed record CqrsAction(string Name, ICqrsSegregation Segregation) : ISegregationAction;
public sealed record CustomAction(string Name, string? CodeStatement) : ICustomAction;
public sealed record CommandCqrsSegregation(string Name, MethodArgument? Parameter = null, MethodArgument? Result = null) : ICommandCqrsSegregation;
public sealed record QueryCqrsSegregation(string Name, MethodArgument? Parameter, MethodArgument? Result) : IQueryCqrsSegregation;