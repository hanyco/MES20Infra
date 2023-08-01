using HanyCo.Infra.Blazor;
using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.CodeGeneration.Helpers;
using HanyCo.Infra.Security.Model;

using Library.CodeGeneration.Models;
using Library.Cqrs.Models.Commands;
using Library.Cqrs.Models.Queries;
using Library.Helpers.CodeGen;
using Library.Validations;

using Microsoft.Extensions.Caching.Memory;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Blazor.Actors;

[Immutable]
[Fluent]
public sealed class BlazorPage : BlazorComponentBase<BlazorPage>
{
    public BlazorPage(in string name, in string? pageRoute = null) : base(name)
        => this.SetPageRoute(pageRoute);

    public string? PageRoute { get; private set; }

    public static BlazorPage New(in string name, in string? pageRoute = null)
        => new(name, pageRoute);

    public BlazorPage SetPageRoute(string? value)
        => this.Fluent(() => this.PageRoute = value);

    protected override StringBuilder OnGeneratingHtmlCode(StringBuilder codeStringBuilder)
    {
        Check.MustBeArgumentNotNull(codeStringBuilder);
        var result = addRequiredNameSpaces(codeStringBuilder);
        return result.AppendLine();

        StringBuilder addRequiredNameSpaces(StringBuilder codeStringBuilder)
        {
            var queryProcessorType = TypePath.New<IQueryProcessor>();
            var commandProcessorType = TypePath.New<ICommandProcessor>();
            var memoryCacheType = TypePath.New<IMemoryCache>();
            var userContextType = TypePath.New<IUserContext>();

            var result = FluencyHelper.With(codeStringBuilder
                .AppendLine(this.PageRoute.IsNullOrEmpty() ? $"@page \"/{this.Name}\"" : $"@page \"{this.PageRoute}\"")
                .AppendLine()
                .AppendLine($"@namespace {this.NameSpace}")
                .AppendLine()
                .AppendLine($"@using {memoryCacheType.NameSpace}")
                .AppendLine($"@using {queryProcessorType.NameSpace}")
                .AppendLine($"@using {commandProcessorType.NameSpace}")
                .AppendLine($"@using {userContextType.NameSpace}").Fluent()
                .IfTrue(!(this.DataContextType?.NameSpace.IsNullOrEmpty() ?? true), sb => sb!.AppendLine($"@using {this.DataContextType!.Value.NameSpace}"))
, appendChildrenNameSpaces).GetValue()!
                .AppendLine()
                .AppendLine($"@inject {queryProcessorType.Name} {TypeMemberNameHelper.ToFieldName(queryProcessorType.Name!)}")
                .AppendLine($"@inject {commandProcessorType.Name} {TypeMemberNameHelper.ToFieldName(commandProcessorType.Name!)}")
                .AppendLine($"@inject {memoryCacheType.Name} {TypeMemberNameHelper.ToFieldName(memoryCacheType.Name!)}")
                .AppendLine($"@inject {userContextType.Name} {TypeMemberNameHelper.ToFieldName(userContextType.Name!)}")
                .AppendLine()
                .AppendLine($"@inherits {TypePath.New<PageBase<int>>().Fluent().IfTrue(this.DataContextType.HasValue, tp => tp.AddGenericType(this.DataContextType!.Value.Name)).Value}")
                .AppendLine();
            return result;

            void appendChildrenNameSpaces(StringBuilder sb) =>
                this.Children.OfType<IBlazorComponent>()
                    .Select(x => x.NameSpace).Compact().Distinct()
                    .ForEachEager(nameSpace => sb.AppendLine($"@using {nameSpace}"));
        }
    }

    protected override void OnInitializingBehindCode(GenerateCodesParameters? arguments)
    {
        this.PartialCodeUsingNameSpaces.Add(typeof(CacheExtensions).Namespace!);
        base.OnInitializingBehindCode(arguments);
    }

    protected override void OnInitializingUiCode(GenerateCodesParameters? arguments)
    {
        CommonExtensions.GetAllChildren(this);
        _ = this.GetAllChildren().OfType<BlazorComponent>().ForEach(x =>
        {
            x.ShouldGenerateFullUiCode = false;
        }).Build();
        base.OnInitializingUiCode(arguments);
    }
}