using System.Diagnostics.CodeAnalysis;

using HanyCo.Infra.CodeGen.Contracts.ViewModels;

using Library.CodeGeneration;
using Library.CodeGeneration.Models;
using Library.Coding;
using Library.Validations;

namespace HanyCo.Infra.CodeGen.Contracts.CodeGen.ViewModels;

public sealed class ApiCodingViewModel : InfraViewModelBase
{
    private string _controllerName;
    private string _controllerRoute;
    private bool _isAnonymousAllow;
    private string _nameSpace;
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

    public HashSet<MethodArgument> Arguments { get; } = [];
    public string? Body { get => this._body; set => this.SetProperty(ref this._body, value); }
    public TypePath? ReturnType { get; set; }
    public string? Route { get; set; }

    public static ApiMethod New([DisallowNull] in string name, in TypePath returnType) => new(name)
    {
        ReturnType = returnType
    };

    public static ApiMethod New([DisallowNull] in string name)
        => new(name);

    public ApiMethod AddArgument(params MethodArgument[] arguments)
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

    public ApiMethod WithBody(string body)
        => this.With(x => x.Body = body);

    public ApiMethod WithReturnType(TypePath type)
        => this.With(x => x.ReturnType = type);
}