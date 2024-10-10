using Library.Data.SqlServer.Dynamics;

using System.Diagnostics.CodeAnalysis;

namespace HanyCo.Infra.CodeGen.Domain.ViewModels;

public sealed class DbColumnViewModel : DbObjectViewModel
{
    public string? Comment { get; init; }
    public string DbType { get; init; }
    public bool IsNullable { get; init; }
    public int? MaxLength { get; init; }

    [SetsRequiredMembers]
    public DbColumnViewModel(string name, long objectId, string dbType, bool isNullable, int? maxLength = null, string? comment = null) : base(name, objectId)
    {
        this.Comment = comment;
        this.DbType = dbType;
        this.IsNullable = isNullable;
        this.MaxLength = maxLength;
    }

    public static DbColumnViewModel FromDbColumn(Column column)
        => new(column.Name, column.UniqueId, column.DataType, column.IsNullable, column.MaxLength);

    public override string ToString()
    {
        var result = $"{this.Name} ({this.DbType}";
        if (this.DbType == "nvarchar")
        {
            result = this.MaxLength == -1 ? $"{result}, max" : $"{result}, {this.MaxLength}";
        }
        if (this.IsNullable)
        {
            result = $"{result}, nullable";
        }

        result = $"{result})";
        return result;
    }
}