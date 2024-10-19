using Library.Data.SqlServer.Dynamics;

using System.Diagnostics.CodeAnalysis;

namespace HanyCo.Infra.CodeGen.Domain.ViewModels;

public sealed class DbColumnViewModel(string name, long objectId, string dbType, bool isNullable, int? maxLength = null, string? comment = null)
    : DbObjectViewModel(name, objectId, null, dbType, comment)
{
    public bool IsIdentity { get; set; }

    public bool IsNullable { get; init; } = isNullable;

    public int? MaxLength { get; init; } = maxLength;

    [return: NotNullIfNotNull(nameof(column))]
    public static DbColumnViewModel? FromDbColumn(Column column) =>
        column is null ? null : new(column.Name, column.UniqueId, column.DataType, column.IsNullable, column.MaxLength) { IsIdentity = column.IsIdentity };

    [return: NotNullIfNotNull(nameof(dbObject))]
    public static DbColumnViewModel? FromDbObjectViewModel(DbObjectViewModel? dbObject, bool isNullable = default, int? maxLength = default) =>
        dbObject is null ? null : new(dbObject.Name, dbObject.Id ?? default, dbObject.Type, isNullable, maxLength, dbObject.Comment);

    public override string ToString()
    {
        var result = $"{this.Name} ({this.Type}";
        if (this.Type == "nvarchar")
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