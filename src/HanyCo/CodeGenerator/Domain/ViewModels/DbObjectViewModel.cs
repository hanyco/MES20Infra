using System.Diagnostics.CodeAnalysis;

namespace HanyCo.Infra.CodeGen.Domain.ViewModels;

public class DbObjectViewModel : InfraViewModelBase
{
    public DbObjectViewModel(string name, long objectId = -1, string? schema = default, string? dbType = default, string? comment = default)
        : base(null, name)
    {
        this.Name = name;
        this.ObjectId = objectId;
        this.Schema = schema;
        this.Type = dbType;
        this.Comment = comment;
    }
    public string? Comment
    {
        get;
        set => this.SetProperty(ref field, value);
    }

    public long ObjectId
    {
        get;
        set => this.SetProperty(ref field, value);
    }
    public string? Schema
    {
        get;
        set => this.SetProperty(ref field, value);
    }
    public string? Type
    {
        get;
        set => this.SetProperty(ref field, value);
    }

    
    public override string ToString() =>
        this.Schema is not null ? $"[{this.Schema}].[{this.Name ?? "No Name!"}]" : $"[{this.Name ?? "No Name!"}]";

    /// <summary>
    /// Format: schema.name.type.dbObjectId
    /// </summary>
    /// <returns></returns>
    [return: MaybeNull]
    public string ToDbFormat() =>
        $"{Schema}.{Name}.{Type}.{ObjectId}";

    [return: NotNullIfNotNull(nameof(dbFormat))]
    public static DbObjectViewModel? FromDbFormat(string? dbFormat)
    {
        if (dbFormat.IsNullOrEmpty())
        {
            return null;
        }
        var parts = dbFormat.Split('.');
        var result = new DbObjectViewModel(parts[1], parts[3].Cast().ToLong(), parts[0], parts[2]);
        return result;
    }
}
