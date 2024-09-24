using HanyCo.Infra.Blazor;
using HanyCo.Infra.CodeGeneration.CodeGenerator.Models;
using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements;

using Library.CodeGeneration;
using Library.CodeGeneration.Models;
using Library.CodeGeneration.v2;
using Library.Cqrs.Models.Commands;
using Library.Cqrs.Models.Queries;
using Library.Helpers.CodeGen;
using Library.Interfaces;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Caching.Memory;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Blazor.Actors;

[Immutable]
[Fluent]
public sealed class BlazorComponent(in string name, ICodeGeneratorEngine codeGenerator) 
    : BlazorComponentBase<BlazorComponent>(name, codeGenerator), IBlazorComponent
{
    public Dictionary<string, string?> BlazorAttributes { get; } = [];
    public (TypePath Type, string Name)? DataContextProperty { get; set; }
    
    public bool ShouldGenerateFullUiCode { get; internal set; } = true;

    public static BlazorComponent New(in string name, in ICodeGeneratorEngine codeGenerator) =>
        new(name, codeGenerator);

    protected override StringBuilder OnGeneratingHtmlCode(StringBuilder codeStringBuilder)
    {
        var injections = new[]
        {
            TypePath.New<IMemoryCache>(),
            //TypePath.New<IUserContext>(),
            TypePath.New<NavigationManager>(),
        };
        var dataContextType = (this.DataContextType, this.DataContextProperty) switch
        {
            (_, { } dc) => dc.Type,
            ({ } dc, null) => dc,
            _ => null
        };
        var componentBaseTypePath = TypePath.New(typeof(ComponentBase<,>).FullName!,
            this.IsGrid
                ? [$"List<{dataContextType.Name}>", this.DataContextType.Name]
                : [$"{dataContextType?.Name}", $"{this.DataContextType?.Name}"]);
        _ = codeStringBuilder
            .AppendLine($"@namespace {this.NameSpace}")
            .AppendLine()
            .AppendAllLines(componentBaseTypePath.GetNameSpaces(), x => $"@using {x}")
            .AppendAllLines(injections, x => $"@using {x.NameSpace}")
            .AppendLine($"@using {this.DataContextType?.NameSpace}")
            .AppendLine("@using Web.UI.Components.Shared")
            .AppendLine()
            .AppendAllLines(injections, x => $"@inject {x.Name} {TypeMemberNameHelper.ToFieldName(x.Name!)}")
            .AppendLine()
            .AppendLine($"@inherits {componentBaseTypePath.FullPath}")
            .AppendLine()
            .AppendLine()
            .AppendLine(@"<MessageComponent @ref=""MessageComponent"" />");
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
            element.Attributes.AddRange(this.Attributes);
            return element.GenerateUiCode();
        }
    }

    protected override void OnInitializingBehindCode(GenerateCodesParameters? arguments)
    {
        this.PartialCodeUsingNameSpaces.Add(typeof(IMemoryCache).Namespace!);
        this.PartialCodeUsingNameSpaces.Add(typeof(NotifyPropertyChanged).Namespace!);
    }
}