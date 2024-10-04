using HanyCo.Infra.CodeGen.Contracts.ViewModels;
using HanyCo.Infra.CodeGeneration.Definitions;

using Library.CodeGeneration;
using Library.CodeGeneration.Models;
using Library.Coding;
using Library.Validations;
using Library.Wpf.Markers;

using Microsoft.AspNetCore.Mvc.Routing;

using System.Diagnostics.CodeAnalysis;

namespace HanyCo.Infra.CodeGen.Domain.ViewModels;

[ViewModel]
public sealed class ApiCodingViewModel : InfraViewModelBase, ICodeBase
{
    private string _controllerName;
    private string _controllerRoute;
    private bool _isAnonymousAllow;
    private string _nameSpace;
    public ISet<string> AdditionalUsings { get; } = new HashSet<string>();
    public HashSet<ApiMethod> Apis { get; } = [];
    public string ControllerName { get => this._controllerName; set => this.SetProperty(ref this._controllerName, value); }
    public string ControllerRoute { get => this._controllerRoute; set => this.SetProperty(ref this._controllerRoute, value); }
    public HashSet<(MethodArgument Argument, bool IsField)> CtorParams { get; } = [];
    public bool IsAnonymousAllow { get => this._isAnonymousAllow; set => this.SetProperty(ref this._isAnonymousAllow, value); }
    public string NameSpace { get => this._nameSpace; set => this.SetProperty(ref this._nameSpace, value); }
}

public sealed class ApiMethod : InfraViewModelBase
{
    private string? _body;

    public ISet<MethodArgument> Arguments { get; } = new HashSet<MethodArgument>();

    public string? Body { get => this._body; set => this.SetProperty(ref this._body, value); }

    public ISet<HttpMethodAttribute> HttpMethods { get; } = new HashSet<HttpMethodAttribute>();

    public bool IsAsync { get; set; }

    public TypePath? ReturnType { get; set; }

    public static ApiMethod New([DisallowNull] in string name)
        => new() { Name = name };
}

public static class ApiExtensions
{
    public static ApiMethod AddArgument(this ApiMethod apiMethod, params IEnumerable<MethodArgument> arguments)
    {
        foreach (var argument in arguments)
        {
            _ = apiMethod.Arguments.Add(argument);
        }

        return apiMethod;
    }

    public static ApiMethod AddArgument(this ApiMethod apiMethod, in TypePath type, in string? name)
        => apiMethod.AddArgument(new MethodArgument(type, name));

    public static ApiMethod AddBodyLine(this ApiMethod apiMethod, string body)
    {
        apiMethod.Body = string.Join(Environment.NewLine, apiMethod.Body, body);
        return apiMethod;
    }

    public static ApiMethod AddHttpMethod(this ApiMethod apiMethod, HttpMethodAttribute httpMethod)
    {
        _ = apiMethod.HttpMethods.Add(httpMethod);
        return apiMethod;
    }

    public static ApiMethod AddHttpMethod<THttpMethod>(this ApiMethod apiMethod, [StringSyntax("Route")] string template)
        where THttpMethod : HttpMethodAttribute
    {
        var ctor = typeof(THttpMethod).GetConstructor([typeof(string)]);
        var method = (THttpMethod)ctor.Invoke([template]);
        return apiMethod.AddHttpMethod(method);
    }

    public static ApiMethod AddHttpMethod<THttpMethod>(this ApiMethod apiMethod)
        where THttpMethod : HttpMethodAttribute, new()
        => apiMethod.AddHttpMethod(new THttpMethod());

    public static ApiMethod IsAsync(this ApiMethod apiMethod, bool isAsync)
    {
        apiMethod.IsAsync = isAsync;
        return apiMethod;
    }

    public static ApiMethod WithReturnType(this ApiMethod apiMethod, TypePath type)
        => apiMethod.With(x => x.ReturnType = type);
}