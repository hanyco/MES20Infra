using HanyCo.Infra.Blazor;
using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements;
using HanyCo.Infra.Security.Model;

using Library.CodeGeneration.Models;
using Library.Cqrs.Models.Commands;
using Library.Cqrs.Models.Queries;
using Library.Helpers.CodeGen;
using Library.Interfaces;

using Microsoft.Extensions.Caching.Memory;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Blazor.Actors;

[Immutable]
[Fluent]
public sealed class BlazorComponent : BlazorComponentBase<BlazorComponent>, IBlazorComponent
{
    public BlazorComponent(in string name) : base(name)
    {
    }

    public Dictionary<string, string?> BlazorAttributes { get; } = new Dictionary<string, string?>();
    public (TypePath Type, string Name)? DataContextProprty { get; private set; }

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

    public static BlazorComponent New(in string name)
        => new(name);

    public BlazorComponent SetDataContextProperty((TypePath Type, string Name)? value)
        => this.This(() => this.DataContextProprty = value);

    public BlazorComponent SetDataContextProperty(TypePath type, string name)
        => this.SetDataContextProperty((type, name));

    protected override StringBuilder OnGeneratingHtmlCode(StringBuilder codeStringBuilder)
    {
        var queryProcessorType = TypePath.New<IQueryProcessor>();
        var commandProcessorType = TypePath.New<ICommandProcessor>();
        var memoryCacheType = TypePath.New<IMemoryCache>();
        var userContextType = TypePath.New<IUserContext>();
        var baseTypeName = typeof(ComponentBase<,>).Name[..^2];
        TypePath? dataContextType = (this.DataContextType, this.DataContextProprty) switch
        {
            (_, { } dc) => dc.Type,
            ({ } dc, null) => dc,
            _ => null
        };
        return codeStringBuilder
                .AppendLine($"@namespace {this.NameSpace}")
                .AppendLine()
                .AppendLine($"@using {memoryCacheType.NameSpace}")
                .AppendLine($"@using {queryProcessorType.NameSpace}")
                .AppendLine($"@using {commandProcessorType.NameSpace}")
                .AppendLine($"@using {userContextType.NameSpace}")
                .AppendLine($"@using {typeof(ComponentBase<,>).Namespace}")
                .AppendLine($"@using {this.DataContextType?.NameSpace}")
                .AppendLine()
                .AppendLine($"@inject {queryProcessorType.Name} {TypeMemberNameHelper.ToFieldName(queryProcessorType.Name!)}")
                .AppendLine($"@inject {commandProcessorType.Name} {TypeMemberNameHelper.ToFieldName(commandProcessorType.Name!)}")
                .AppendLine($"@inject {memoryCacheType.Name} {TypeMemberNameHelper.ToFieldName(memoryCacheType.Name!)}")
                .AppendLine($"@inject {userContextType.Name} {TypeMemberNameHelper.ToFieldName(userContextType.Name!)}")
                .AppendLine()
                .Fluent()
                .If(this.IsGrid
                    , sb => sb.AppendLine($"@inherits {baseTypeName}<List<{dataContextType?.Name}>, {this.DataContextType?.Name}>")
                    , sb => sb.AppendLine($"@inherits {baseTypeName}<{dataContextType?.Name},{this.DataContextType?.Name}>"))
                .GetValue()!
                .AppendLine();
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
            if (this.Position?.Col is not null and not 0 and not 1)
            {
                element.Attributes.Add($"col-{this.Position.Col}", null);
            }
            if (this.DataContextProprty is { } prop)
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