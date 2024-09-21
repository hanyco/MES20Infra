using System.Diagnostics.CodeAnalysis;

using HanyCo.Infra.Blazor;
using HanyCo.Infra.CodeGeneration.CodeGenerator.Models;
using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.CodeGeneration.Helpers;

using Library.CodeGeneration;
using Library.CodeGeneration.Models;
using Library.Cqrs.Models.Commands;
using Library.Cqrs.Models.Queries;
using Library.Helpers.CodeGen;
using Library.Validations;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Caching.Memory;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Blazor.Actors;

[Immutable]
[Fluent]
public sealed class BlazorPage : BlazorComponentBase<BlazorPage>
{
    private BlazorPage(in string name, in string? moduleName = null, in IEnumerable<string>? pageRoutes = null) : base(name)
    {
        _ = this.SetPageRoutes(pageRoutes);
        this.ModuleName = moduleName;
    }

    public string? ModuleName { get; }
    public IEnumerable<string>? PageRoutes { get; private set; }

    [return: NotNull]
    public static string GetPageRoute(string pageName, string? moduleName, string? pageRoute, params string[] parameters)
    {
        string? pureRoute;
        if (pageRoute.IsNullOrEmpty())
        {
            // Generate route form scratch.
            pureRoute = $"@page \"/{moduleName.ArgumentNotNull().Remove(" ")}/{purify(pageName)}";
        }
        else
        {
            if (pageRoute.StartsWith("@page "))
            {
                // The input value is a complete route. Not required to be reformatted. Just add
                // parameters to the end of the route, if any.
                pureRoute = pageRoute.TrimEnd('\"');
            }
            else
            {
                pureRoute = $"@page \"/{purify(pageRoute)}";
            }
        }

        var fixedParameters = parameters.Select(x => x.TrimStart('{').TrimEnd('}').Format(x => $"{{{x}}}"));
        var result = merge(pureRoute, fixedParameters).AddEnd("\"");

        return result;

        static string purify(string pageName) => pageName.NotNull().TrimStart('/').TrimEnd('/').TrimSuffix("Page").Trim();
        static string merge(string s, IEnumerable<string> items)
        {
            var result = new StringBuilder(s);
            items.ForEach(item => result.Append($"/{item}"));
            return result.ToString();
        }
    }

    [return: NotNull]
    public static BlazorPage NewByModuleName([DisallowNull] in string name, [DisallowNull] in string moduleName) =>
        new(name.NotNull(), moduleName: moduleName.NotNull());

    [return: NotNull]
    public static BlazorPage NewByPageRoute([DisallowNull] in string name, [DisallowNull] in IEnumerable<string> pageRoutes) =>
        new(name.NotNull(), pageRoutes: pageRoutes);

    public BlazorPage SetPageRoutes(IEnumerable<string>? value) =>
        this.Fluent(() => this.PageRoutes = value);

    protected override StringBuilder OnGeneratingHtmlCode(StringBuilder codeStringBuilder)
    {
        Check.MustBeArgumentNotNull(codeStringBuilder);
        return addHeaders(codeStringBuilder).AppendLine();

        StringBuilder addHeaders(StringBuilder codeStringBuilder)
        {
            var injections = new[]
            {
                TypePath.New<IMemoryCache>(),
                //TypePath.New<IUserContext>(),
                TypePath.New<NavigationManager>(),
            };

            var pageRoute = this.PageRoutes;
            var moduleName = this.ModuleName;
            var name = this.Name;
            var routes = pageRoute?.Select(pageRoute => GetPageRoute(name, moduleName, pageRoute));

            var result = codeStringBuilder
                .AppendAllLines(routes)
                .AppendLine()
                .AppendLine($"@namespace {this.NameSpace}")
                .AppendLine()
                .AppendAllLines(injections, x => $"@using {x.NameSpace}");
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
                .AppendAllLines(injections, x => $"@inject {x.Name} {TypeMemberNameHelper.ToFieldName(x.Name!)}")
                .AppendLine();
            List<string> generics = [];
            if (this.DataContextType is { } dct2)
            {
                generics.Add(dct2);
            };
            var inherits = TypePath.New(typeof(PageBase<>).FullName!, generics).FullPath;
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