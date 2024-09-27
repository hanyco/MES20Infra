using HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements;

using Library.CodeGeneration;
using Library.Interfaces;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

public interface IActionCodesGenerate
{
    IEnumerable<ClassMembers>? GenerateCodeTypeMembers();
}

public interface IBlazorComponent : IHtmlElement
{
    Dictionary<string, string?> BlazorAttributes { get; }
    
    string? NameSpace { get; }
}

public interface ICustomAction
{
    string? CodeStatement { get; }
    string Name { get; }
}

public interface IHasCustomAction : IActionCodesGenerate
{
    ICustomAction? Action { get; }
}

public interface IHasInnerText
{
    string? InnerText { get; set; }
}

public interface IHasPosition
{
    public BootstrapPosition Position { get; }
}

public interface IHasSegregationAction : IActionCodesGenerate
{
    ISegregationAction? Action { get; }
}

public interface ISegregationAction
{
    string Name { get; }

    ICqrsSegregation Segregation { get; }
}

public interface IHtmlElement : IUiCodeGenerator, IHasPosition, IParent<IHtmlElement>
{
    Dictionary<string, string?> Attributes { get; }

    string? Name { get; }
}