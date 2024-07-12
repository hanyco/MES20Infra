using System.Diagnostics.CodeAnalysis;

using HanyCo.Infra.CodeGeneration.CodeGenerator.Models;
using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements;

using Library.CodeGeneration.Models;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Blazor.Components;

public sealed class ValidationMessage : IBlazorComponent
{
    public ValidationMessage()
    {
    }

    public ValidationMessage(string forValue, string? cssClass) =>
        (this.ForValue, this.CssClass) = (forValue, cssClass);

    public Dictionary<string, string?> Attributes { get; } = [];

    public Dictionary<string, string?> BlazorAttributes { get; } = [];

    public IList<IHtmlElement> Children { get; } = new List<IHtmlElement>();

    public string? CssClass { get; set; }

    public string? ForValue { get; set; }

    [NotNull]
    public string? Name { get; } = nameof(ValidationMessage);

    public string? NameSpace { get; } = "Microsoft.AspNetCore.Components.Forms";

    public BootstrapPosition Position { get; } = new();

    public Code GenerateUiCode(GenerateCodesParameters? arguments = null)
    {
        StringBuilder statement = new($"<ValidationMessage For=\"@(() => {this.ForValue})\"");
        if (!this.CssClass.IsNullOrEmpty())
        {
            _ = statement.Append($" Class=\"{this.CssClass}\"");
        }
        _ = statement.Append(" />");

        return new(this.Name, Languages.BlazorFront, statement.ToString());
    }
}