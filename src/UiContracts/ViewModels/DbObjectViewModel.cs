using Contracts.ViewModels;

namespace HanyCo.Infra.UI.ViewModels;

public class DbObjectViewModel : InfraViewModelBase
{
    private long _objectId;
    private string? _schema;

    public DbObjectViewModel(string name, long objectId = -1, string? schema = null)
        : base(null, name)
    {
        this.Name = name;
        this.ObjectId = objectId;
        this.Schema = schema;
    }

    public long ObjectId
    {
        get => this._objectId;
        set => this.SetProperty(ref this._objectId, value);
    }
    public string? Schema
    {
        get => this._schema;
        set => this.SetProperty(ref this._schema, value);
    }

    public override string ToString() =>
        this.Schema is not null ? $"{this.Schema}.{this.Name ?? "No Name!"}" : this.Name ?? "No Name!";
}
