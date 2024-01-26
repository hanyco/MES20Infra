using Library.CodeGeneration;

namespace HanyCo.Infra.CodeGen.Contracts.ViewModels;

public sealed class ApiCodingViewModel : InfraViewModelBase
{
    private string _nameSpace;

    public ApiMethod Delete { get; set; } = new();
    public string DtoType { get; set; }
    public ApiMethod GetAllApi { get; } = new();
    public ApiMethod GetById { get; } = new();
    public string NameSpace { get => this._nameSpace; set => this.SetProperty(ref this._nameSpace, value); }
    public ApiMethod Post { get; } = new();
    public ApiMethod Put { get; } = new();
    public string ControllerRoute { get; set; }
    public string ControllerName { get; set; }
}

public sealed class ApiMethod
{
    public string? Body { get; set; }
    public HashSet<(TypePath Type, string Name)> Parameters { get; } = [];
    public TypePath? ReturnType { get; set; }
    //public string? Route { get; set;}
    //public string Name { get; set; }
}