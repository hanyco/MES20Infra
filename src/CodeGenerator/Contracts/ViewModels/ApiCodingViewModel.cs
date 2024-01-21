namespace Contracts.ViewModels;

public sealed class ApiCodingViewModel : InfraViewModelBase
{
    private string _nameSpace;
    private string _typeName;

    public string NameSpace { get => this._nameSpace; set => this.SetProperty(ref this._nameSpace, value); }

    public string TypeName { get => this._typeName; set => this.SetProperty(ref this._typeName, value); }
}