using HanyCo.Infra.CodeGeneration.Definitions;
using HanyCo.Infra.Security;

using Library.CodeGeneration;
using Library.CodeGeneration.Models;
using Library.Coding;
using Library.Wpf.Markers;

using Microsoft.AspNetCore.Mvc.Routing;

using System.Diagnostics.CodeAnalysis;

namespace HanyCo.Infra.CodeGen.Domain.ViewModels;

public sealed class ControllerMethodViewModel : InfraViewModelBase
{
    public ISet<MethodArgument> Arguments { get; } = new HashSet<MethodArgument>();

    public string? Body { get; set => this.SetProperty(ref field, value); }

    public ISet<HttpMethodAttribute> HttpMethods { get; } = new HashSet<HttpMethodAttribute>();
    
    public string? PermissionValue { get; set => this.SetProperty(ref field, value); }

    public bool IsAsync { get; set; }

    public TypePath? ReturnType { get; set; }

    public static ControllerMethodViewModel New([DisallowNull] in string name) => 
        new() { Name = name };
}

[ViewModel]
public sealed class ControllerViewModel : InfraViewModelBase, ICodeBase
{
    private bool _isAnonymousAllow;
    private string _nameSpace;
    private string _route;
    public ISet<string> AdditionalUsings { get; } = new HashSet<string>();
    public HashSet<ControllerMethodViewModel> Apis { get; } = [];
    public HashSet<(MethodArgument Argument, bool IsField)> CtorParams { get; } = [];
    public bool IsAnonymousAllow { get => this._isAnonymousAllow; set => this.SetProperty(ref this._isAnonymousAllow, value); }
    public ModuleViewModel Module { get; set; }
    public string NameSpace { get => this._nameSpace; set => this.SetProperty(ref this._nameSpace, value); }
    public string Route { get => this._route; set => this.SetProperty(ref this._route, value); }
}

public static class ApiExtensions
{
    public static ControllerMethodViewModel AddArgument(this ControllerMethodViewModel apiMethod, params IEnumerable<MethodArgument> arguments)
    {
        foreach (var argument in arguments)
        {
            _ = apiMethod.Arguments.Add(argument);
        }

        return apiMethod;
    }

    public static ControllerMethodViewModel AddArgument(this ControllerMethodViewModel apiMethod, in TypePath type, in string? name)
        => apiMethod.AddArgument(new MethodArgument(type, name));

    public static ControllerMethodViewModel AddBodyLine(this ControllerMethodViewModel apiMethod, string body)
    {
        apiMethod.Body = string.Join(Environment.NewLine, apiMethod.Body, body);
        return apiMethod;
    }

    public static ControllerMethodViewModel AddHttpMethod(this ControllerMethodViewModel apiMethod, HttpMethodAttribute httpMethod)
    {
        _ = apiMethod.HttpMethods.Add(httpMethod);
        return apiMethod;
    }

    // Add PermissionAttribute
    public static ControllerMethodViewModel AddPermission(this ControllerMethodViewModel apiMethod, string permissionValue)
    {
        _ = apiMethod.PermissionValue = permissionValue;
        return apiMethod;
    }


    public static ControllerMethodViewModel AddHttpMethod<THttpMethod>(this ControllerMethodViewModel apiMethod, [StringSyntax("Route")] string template)
        where THttpMethod : HttpMethodAttribute
    {
        var ctor = typeof(THttpMethod).GetConstructor([typeof(string)]);
        var method = (THttpMethod)ctor.Invoke([template]);
        return apiMethod.AddHttpMethod(method);
    }

    public static ControllerMethodViewModel AddHttpMethod<THttpMethod>(this ControllerMethodViewModel apiMethod)
        where THttpMethod : HttpMethodAttribute, new()
        => apiMethod.AddHttpMethod(new THttpMethod());

    public static ControllerMethodViewModel IsAsync(this ControllerMethodViewModel apiMethod, bool isAsync)
    {
        apiMethod.IsAsync = isAsync;
        return apiMethod;
    }

    public static ControllerMethodViewModel WithReturnType(this ControllerMethodViewModel apiMethod, TypePath type)
        => apiMethod.With(x => x.ReturnType = type);
}