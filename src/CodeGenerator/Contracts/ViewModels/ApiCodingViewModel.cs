using System.Diagnostics.CodeAnalysis;

using Library.CodeGeneration;
using Library.CodeGeneration.Models;
using Library.Coding;
using Library.Validations;

namespace HanyCo.Infra.CodeGen.Contracts.ViewModels;

public sealed class ApiCodingViewModel : InfraViewModelBase
{
    private string _controllerName;
    private string _controllerRoute;
    private string _nameSpace;
    public HashSet<ApiMethod> Apis { get; } = [];
    public string ControllerName { get => _controllerName; set => this.SetProperty(ref this._controllerName, value); }
    public string ControllerRoute { get => _controllerRoute; set => this.SetProperty(ref this._controllerRoute, value); }
    public HashSet<(MethodArgument Argument, bool IsField)> CtorParams { get; } = [];
    public string NameSpace { get => this._nameSpace; set => this.SetProperty(ref this._nameSpace, value); }
}

public sealed class ApiMethod : InfraViewModelBase
{
    private string? _body;

    public ApiMethod(string name)
    {
        this.Name = name.ArgumentNotNull();
    }

    public string? Body { get => _body; set => this.SetProperty(ref this._body, value); }

    public HashSet<MethodArgument> Parameters { get; } = [];

    public TypePath? ReturnType { get; set; }

    public string? Route { get; set; }

    public static ApiMethod New([DisallowNull] in string name, in TypePath returnType) => new(name)
    {
        ReturnType = returnType
    };

    public static ApiMethod New([DisallowNull] in string name)
        => new(name);

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