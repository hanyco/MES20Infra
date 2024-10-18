using Library.Data.SqlServer.Dynamics;

namespace HanyCo.Infra.CodeGen.Domain.ViewModels;

public sealed class DbColumnViewModel(string name, long objectId, string dbType, bool isNullable, int? maxLength = null, string? comment = null)
    : DbObjectViewModel(name, objectId, null, dbType)
{
    public string? Comment { get; set; } = comment;
    public bool IsNullable { get; init; } = isNullable;
    public int? MaxLength { get; init; } = maxLength;

    public static DbColumnViewModel FromDbColumn(Column column)
        => new(column.Name, column.UniqueId, column.DataType, column.IsNullable, column.MaxLength);

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