using System.Diagnostics.CodeAnalysis;

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
    private readonly string? _moduleName;

    private BlazorPage(in string name, in string? moduleName = null, in string? pageRoute = null) : base(name)
    {
        _ = this.SetPageRoute(pageRoute);
        this._moduleName = moduleName;
    }

    public string? PageRoute { get; private set; }

    [return: NotNull]
    public static string GetPageRoute(string name, string? moduleName, string? pageRoute) =>
        pageRoute.IsNullOrEmpty()
            ? $"@page \"/{moduleName.NotNull().Remove(" ")}/{name.NotNull().TrimEnd("Page", StringComparison.OrdinalIgnoreCase).TrimStart('/')}\""
            : $"@page \"/{pageRoute.TrimStart('/')}\"";

    [return: NotNull]
    public static BlazorPage NewByModuleName([DisallowNull] in string name, [DisallowNull] in string moduleName) =>
        new(name.NotNull(), moduleName: moduleName.NotNull());

    [return: NotNull]
    public static BlazorPage NewByPageRoute([DisallowNull] in string name, [DisallowNull] in string pageRoute) =>
        new(name.NotNull(), pageRoute: pageRoute.NotNull());

    public BlazorPage SetPageRoute(string? value) =>
        this.Fluent(() => this.PageRoute = value);

    protected override StringBuilder OnGeneratingHtmlCode(StringBuilder codeStringBuilder)
    {
        Check.MustBeArgumentNotNull(codeStringBuilder);
        return addHeaders(codeStringBuilder).AppendLine();

        StringBuilder addHeaders(StringBuilder codeStringBuilder)
        {
            var queryProcessorType = TypePath.New<IQueryProcessor>();
            var commandProcessorType = TypePath.New<ICommandProcessor>();
            var memoryCacheType = TypePath.New<IMemoryCache>();
            var userContextType = TypePath.New<IUserContext>();

            var pageRoute = this.PageRoute;
            var moduleName = this._moduleName;
            var name = this.Name;
            var route = GetPageRoute(name, moduleName, pageRoute);

            var result = codeStringBuilder
                .AppendLine(route)
                .AppendLine()
                .AppendLine($"@namespace {this.NameSpace}")
                .AppendLine()
                .AppendLine($"@using {memoryCacheType.NameSpace}")
                .AppendLine($"@using {queryProcessorType.NameSpace}")
                .AppendLine($"@using {commandProcessorType.NameSpace}")
                .AppendLine($"@using {userContextType.NameSpace}");
            if (this.DataContextType is { } dct1 && !dct1.NameSpace.IsNullOrEmpty())
            {
                _ = codeStringBuilder.AppendLine($"@using {dct1.NameSpace}");
            }
            else
            {
                this.Children.OfType<IBlazorComponent>()
                    .Select(x => x.NameSpace).Compact().Distinct()
                    .ForEach(nameSpace => codeStringBuilder.AppendLine($"@using {nameSpace}"));
            }

            _ = codeStringBuilder.AppendLine()
                .AppendLine("@inject NavigationManager NavigationManager")
                .AppendLine($"@inject {memoryCacheType.Name} {TypeMemberNameHelper.ToFieldName(memoryCacheType.Name!)}")
                .AppendLine($"@inject {userContextType.Name} {TypeMemberNameHelper.ToFieldName(userContextType.Name!)}")
                .AppendLine($"@inject {queryProcessorType.Name} {TypeMemberNameHelper.ToFieldName(queryProcessorType.Name!)}")
                .AppendLine($"@inject {commandProcessorType.Name} {TypeMemberNameHelper.ToFieldName(commandProcessorType.Name!)}")
                .AppendLine();
            var inherits = TypePath.New<PageBase<int>>();

            if (this.DataContextType is { } dct2)
            {
                inherits = inherits.AddGenericType(dct2.Name);
            }

            _ = codeStringBuilder.AppendLine($"@inherits {inherits}");
            _ = codeStringBuilder.AppendLine();
            return result;
        }
    }

    protected override void OnInitializingBehindCode(GenerateCodesParameters? arguments)
    {
        this.PartialCodeUsingNameSpaces.Add(typeof(CacheExtensions).Namespace!);
        base.OnInitializingBehindCode(arguments);
    }

    protected override void OnInitializingUiCode(GenerateCodesParameters? arguments)
    {
        this.GetAllChildren().OfType<BlazorComponent>().ForEach(x =>
        {
            x.ShouldGenerateFullUiCode = false;
        });
        base.OnInitializingUiCode(arguments);
    }
}