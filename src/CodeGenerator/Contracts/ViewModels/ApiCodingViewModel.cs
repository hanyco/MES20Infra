using Library.CodeGeneration;

namespace Contracts.ViewModels;

public sealed class ApiCodingViewModel : InfraViewModelBase
{
    private string _nameSpace;

    public ApiMethod? Delete { get; set; }
    public string DtoType { get; set; }
    public ApiMethod? GetAllApi { get; set; }
    public ApiMethod? GetById { get; set; }
    public string NameSpace { get => this._nameSpace; set => this.SetProperty(ref this._nameSpace, value); }
    public ApiMethod? Post { get; set; }
    public ApiMethod? Put { get; set; }
    public string? Route { get; set; }
}

public sealed class ApiMethod
{
    public string? Body { get; set; }
    public HashSet<(TypePath Type, string Name)> Parameters { get; } = [];
    public TypePath? ReturnType { get; set; }
}