using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using HanyCo.Infra.CodeGen.Contracts.ViewModels;
using HanyCo.Infra.CodeGeneration.Definitions;

using Library.CodeGeneration;
using Library.CodeGeneration.Models;
using Library.Coding;
using Library.Validations;

using Microsoft.AspNetCore.Mvc.Routing;

namespace HanyCo.Infra.CodeGen.Contracts.CodeGen.ViewModels;

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

    public ApiMethod(string name)
        => this.Name = name.ArgumentNotNull();

    public ISet<MethodArgument> Arguments { get; } = new HashSet<MethodArgument>();
    public string? Body { get => this._body; set => this.SetProperty(ref this._body, value); }
    public ISet<HttpMethodAttribute> HttpMethods { get; } = new HashSet<HttpMethodAttribute>();

    private bool _isAsync;

    public ApiMethod IsAsync(bool isAsync)
    {
        this._isAsync = isAsync;
        return this;
    }
    public bool IsAsync() => _isAsync;

    public TypePath? ReturnType { get; set; }

    public static ApiMethod New([DisallowNull] in string name, in TypePath returnType)
        => new(name) { ReturnType = returnType };

    public static ApiMethod New([DisallowNull] in string name)
        => new(name);

    public ApiMethod AddArgument(params IEnumerable<MethodArgument> arguments)
    {
        foreach (var argument in arguments)
        {
            _ = this.Arguments.Add(argument);
        }

        return this;
    }

    public ApiMethod AddArgument(in TypePath type, in string? name)
        => this.AddArgument(new MethodArgument(type, name));

    public ApiMethod AddBodyLine(string body)
    {
        this.Body = string.Join(Environment.NewLine, this.Body, body);
        return this;
    }

    public ApiMethod AddHttpMethod(HttpMethodAttribute httpMethod)
    {
        _ = this.HttpMethods.Add(httpMethod);
        return this;
    }

    public ApiMethod AddHttpMethod<THttpMethod>([StringSyntax("Route")] string template)
        where THttpMethod : HttpMethodAttribute
    {
        var ctor = typeof(THttpMethod).GetConstructor([typeof(string)]);
        var method = (THttpMethod)ctor.Invoke([template]);
        return this.AddHttpMethod(method);
    }

    public ApiMethod AddHttpMethod<THttpMethod>()
        where THttpMethod : HttpMethodAttribute, new()
        => this.AddHttpMethod(new THttpMethod());

    public ApiMethod WithBody(string body)
        => this.With(x => x.Body = body);

    public ApiMethod WithReturnType(TypePath type)
        => this.With(x => x.ReturnType = type);
}