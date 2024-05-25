using Library.CodeGeneration;

namespace HanyCo.Infra.CodeGen.Contracts.ViewModels;

public sealed class ApiCodingViewModel : InfraViewModelBase
{
    private string _controllerName;
    private string _controllerRoute;
    private string _nameSpace;
    public List<ApiMethod> Apis { get; } = [];
    public string ControllerName { get => _controllerName; set => this.SetProperty(ref this._controllerName, value); }
    public string ControllerRoute { get => _controllerRoute; set => this.SetProperty(ref this._controllerRoute, value); }
    public string DtoType { get; set; }
    public string NameSpace { get => this._nameSpace; set => this.SetProperty(ref this._nameSpace, value); }
}

public sealed class ApiMethod : InfraViewModelBase
{
    private string? _body;

    public string? Body { get => _body; set => this.SetProperty(ref this._body, value); }
    public HashSet<(TypePath Type, string Name)> Parameters { get; } = [];
    public TypePath? ReturnType { get; set; }
    public string? Route { get; set; }
}