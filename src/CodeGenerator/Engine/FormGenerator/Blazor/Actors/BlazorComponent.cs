using HanyCo.Infra.Blazor;
using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements;

using Library.CodeGeneration.Models;
using Library.Cqrs.Models.Commands;
using Library.Cqrs.Models.Queries;
using Library.Helpers.CodeGen;
using Library.Interfaces;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Caching.Memory;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Blazor.Actors;

[Immutable]
[Fluent]
public sealed class BlazorComponent(in string name) : BlazorComponentBase<BlazorComponent>(name), IBlazorComponent
{
    public Dictionary<string, string?> BlazorAttributes { get; } = new Dictionary<string, string?>();
    public (TypePath Type, string Name)? DataContextProperty { get; set; }

    /// <summary>
    /// Gets a value indicating whether [should generate full UI code].
    /// </summary>
    /// <value><c>true</c> if [should generate full UI code]; otherwise, <c>false</c>.</value>
    /// <remarks>
    /// When we want to generated component code lonely, the code must be fully generated. But if
    /// the component must be inserted in a page, the code mustn't be fully generated. Instead, we
    /// need a simple tag
    /// </remarks>
    public bool ShouldGenerateFullUiCode { get; internal set; } = true;

    public static BlazorComponent New(in string name) =>
        new(name);

    protected override StringBuilder OnGeneratingHtmlCode(StringBuilder codeStringBuilder)
    {
        var injections = new[]
        {
            TypePath.New<IQueryProcessor>(),
            TypePath.New<ICommandProcessor>(),
            TypePath.New<IMemoryCache>(),
            //TypePath.New<IUserContext>(),
            TypePath.New<NavigationManager>(),
        };
        _ = codeStringBuilder
            .AppendLine($"@namespace {this.NameSpace}")
            .AppendLine()
            .AppendAllLines(injections, x => $"@using {x.NameSpace}")
            .AppendLine($"@using {typeof(ComponentBase<,>).Namespace}")
            .AppendLine($"@using {this.DataContextType?.NameSpace}")
            .AppendLine()
            .AppendAllLines(injections, x => $"@inject {x.Name} {TypeMemberNameHelper.ToFieldName(x.Name!)}")
            .AppendLine();

        var baseTypeName = typeof(ComponentBase<,>).Name[..^2];
        var dataContextType = (this.DataContextType, this.DataContextProperty) switch
        {
            (_, { } dc) => dc.Type,
            ({ } dc, null) => dc,
            _ => null
        };
        _ = this.IsGrid
            ? codeStringBuilder.AppendLine($"@inherits {baseTypeName}<List<{dataContextType?.Name}>, {this.DataContextType?.Name}>")
            : codeStringBuilder.AppendLine($"@inherits {baseTypeName}<{dataContextType?.Name},{this.DataContextType?.Name}>");
        _ = codeStringBuilder.AppendLine();
        return codeStringBuilder;
    }

    protected override Code OnGeneratingUiCode(in GenerateCodesParameters? arguments = null)
    {
        if (this.ShouldGenerateFullUiCode)
        {
            return base.OnGeneratingUiCode(arguments);
        }
        else
        {
            var element = new HtmlElement(this.Name);
            if (this.Position?.Col is not null and not 0)
            {
                element.Attributes.Add($"col-{this.Position.Col}", null);
            }
            if (this.DataContextProperty is { } prop)
            {
                element.Attributes.Add("DataContext", $"@this.DataContext?.{prop.Name}");
            }
            else if (this.DataContextType is not null)
            {
                element.Attributes.Add("DataContext", "@this.DataContext");
            }
            element.Attributes.Add("PageDataContext", "@this.DataContext");
            return element.GenerateUiCode();
        }
    }

    protected override void OnInitializingBehindCode(GenerateCodesParameters? arguments)
    {
        this.PartialCodeUsingNameSpaces.Add(typeof(IMemoryCache).Namespace!);
        this.PartialCodeUsingNameSpaces.Add(typeof(NotifyPropertyChanged).Namespace!);
    }
}