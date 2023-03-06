using HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements;

using Library.Interfaces;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

public interface IBlazorComponent : IHtmlElement
{
    Dictionary<string, string?> BlazorAttributes { get; }

    string? NameSpace { get; }
}

public interface IHasHtmlAction
{
    IHtmlAction Action { get; }

    IEnumerable<GenerateCodeTypeMemberResult>? GenerateActionCodes();
}

public interface IHasInnerText
{
    string? InnerText { get; set; }
}

public interface IHasPosition
{
    public BootstrapPosition Position { get; }
}

public interface IHtmlAction
{
    string Name { get; }

    ICqrsSegregation Segregation { get; }
}

public interface IHtmlElement : IUiCodeGenerator, IHasPosition, IParent<IHtmlElement>
{
    Dictionary<string, string?> Attributes { get; }

    string? Name { get; }
}